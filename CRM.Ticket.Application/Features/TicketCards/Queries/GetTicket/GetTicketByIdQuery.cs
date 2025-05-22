using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Common.Persistence;
using CRM.Ticket.Domain.Common.Models;
using CRM.Ticket.Domain.Entities.Comments;
using CRM.Ticket.Domain.Entities.Tickets;
using CRM.Ticket.Domain.Entities.Tickets.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRM.Ticket.Application.Features.TicketCards.Queries.GetTicket;

public sealed record GetTicketByIdQuery(Guid Id) : IRequest<TicketDetailDto>;

public record TicketDetailDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TicketPriority Priority { get; init; }
    public TicketStatus Status { get; init; }
    public TicketType Type { get; init; }
    public Guid CustomerId { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string CategoryColor { get; init; } = string.Empty;
    public DateTimeOffset? DueDate { get; init; }
    public DateTimeOffset? ResolvedAt { get; init; }
    public DateTimeOffset? ClosedAt { get; init; }
    public string? ResolutionNotes { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public List<string> Tags { get; init; } = new();
    public int ViewCount { get; init; }
    public int CommentCount { get; init; }
    public int AttachmentCount { get; init; }
    public DateTimeOffset? FirstResponseAt { get; init; }
    public List<TicketCommentDto> Comments { get; init; } = new();
    public List<TicketAttachmentDto> Attachments { get; init; } = new();
    public List<TicketStatusHistoryDto> StatusHistory { get; init; } = new();
}

public record TicketCommentDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    public bool IsInternal { get; init; }
    public bool IsEdited { get; init; }
    public DateTimeOffset? EditedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public Guid? ParentCommentId { get; init; }
    public List<TicketCommentDto> Replies { get; init; } = new();
}

public record TicketAttachmentDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public Guid UploadedBy { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public record TicketStatusHistoryDto
{
    public Guid Id { get; init; }
    public TicketStatus FromStatus { get; init; }
    public TicketStatus ToStatus { get; init; }
    public string? Reason { get; init; }
    public Guid ChangedBy { get; init; }
    public DateTimeOffset ChangedAt { get; init; }
}

public class GetTicketByIdQueryHandler(IRepository<TicketCard> _ticketRepository, IUnitOfWork _unitOfWork) : IRequestHandler<GetTicketByIdQuery, TicketDetailDto>
{
    public async ValueTask<Result<TicketDetailDto>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetQueryable()
            .Include(t => t.Category)
            .Include(t => t.Metadata)
            .Include(t => t.Comments.OrderBy(c => c.CreatedAt))
                .ThenInclude(c => c.Replies.OrderBy(r => r.CreatedAt))
            .Include(t => t.Attachments.OrderBy(a => a.CreatedAt))
            .Include(t => t.StatusHistory.OrderBy(sh => sh.ChangedAt))
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (ticket == null)
            return Result.Failure<TicketDetailDto>("Can't find ticket", "NotFound");

        ticket.IncrementViewCount();
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = MapToDetailDto(ticket);
        return Result.Success(dto);
    }

    private static TicketDetailDto MapToDetailDto(TicketCard ticket)
    {
        return new TicketDetailDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Status = ticket.Status,
            Type = ticket.Type,
            CustomerId = ticket.CustomerId,
            AssignedToUserId = ticket.AssignedToUserId,
            CategoryId = ticket.CategoryId,
            CategoryName = ticket.Category.Name,
            CategoryColor = ticket.Category.Color,
            DueDate = ticket.DueDate,
            ResolvedAt = ticket.ResolvedAt,
            ClosedAt = ticket.ClosedAt,
            ResolutionNotes = ticket.ResolutionNotes,
            CreatedAt = ticket.CreatedAt,
            Tags = ticket.Tags,
            ViewCount = ticket.Metadata.ViewCount,
            CommentCount = ticket.Metadata.CommentCount,
            AttachmentCount = ticket.Metadata.AttachmentCount,
            FirstResponseAt = ticket.Metadata.FirstResponseAt,
            Comments = ticket.Comments.Where(c => c.ParentCommentId == null).Select(MapComment).ToList(),
            Attachments = ticket.Attachments.Select(a => new TicketAttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                UploadedBy = a.UploadedBy,
                CreatedAt = a.CreatedAt
            }).ToList(),
            StatusHistory = ticket.StatusHistory.Select(sh => new TicketStatusHistoryDto
            {
                Id = sh.Id,
                FromStatus = sh.FromStatus,
                ToStatus = sh.ToStatus,
                Reason = sh.Reason,
                ChangedBy = sh.ChangedBy,
                ChangedAt = sh.ChangedAt
            }).ToList()
        };
    }

    private static TicketCommentDto MapComment(TicketComment comment)
    {
        return new TicketCommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            AuthorId = comment.AuthorId,
            IsInternal = comment.IsInternal,
            IsEdited = comment.IsEdited,
            EditedAt = comment.EditedAt,
            CreatedAt = comment.CreatedAt,
            ParentCommentId = comment.ParentCommentId,
            Replies = comment.Replies.Select(MapComment).ToList()
        };
    }
}