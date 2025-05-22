using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Common.Models.Grids;
using CRM.Ticket.Application.Common.Services.Grids;
using CRM.Ticket.Domain.Common.Models;
using CRM.Ticket.Domain.Entities.Tickets;
using CRM.Ticket.Domain.Entities.Tickets.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRM.Ticket.Application.Features.TicketCards.Queries.GetTickets;

public sealed class GetTicketsQuery : GridQueryBase, IRequest<GridResponse<TicketDto>>
{
    public TicketStatus? Status { get; init; }
    public TicketPriority? Priority { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public string? SearchTerm { get; init; }
}

public record TicketDto
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
    public DateTimeOffset CreatedAt { get; init; }
    public List<string> Tags { get; init; } = new();
    public int ViewCount { get; init; }
    public int CommentCount { get; init; }
    public int AttachmentCount { get; init; }
}

public class GetTicketsQueryHandler(IRepository<TicketCard> _ticketRepository, IGridService _gridService) : IRequestHandler<GetTicketsQuery, GridResponse<TicketDto>>
{
    public async ValueTask<Result<GridResponse<TicketDto>>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _ticketRepository.GetQueryable()
                .Include(t => t.Category)
                .Include(t => t.Metadata)
                .AsQueryable();

            query = ApplyFilters(query, request);

            var result = await _gridService.ProcessGridQuery(
                query,
                request,
                MapToDto,
                cancellationToken);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<GridResponse<TicketDto>>(ex.Message);
        }
    }

    private static IQueryable<TicketCard> ApplyFilters(IQueryable<TicketCard> query, GetTicketsQuery request)
    {
        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.Priority.HasValue)
            query = query.Where(t => t.Priority == request.Priority.Value);

        if (request.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == request.CategoryId.Value);

        if (request.AssignedToUserId.HasValue)
            query = query.Where(t => t.AssignedToUserId == request.AssignedToUserId.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(t =>
                t.Title.Contains(request.SearchTerm) ||
                t.Description.Contains(request.SearchTerm));
        }

        return query;
    }

    private static TicketDto MapToDto(TicketCard ticket)
    {
        return new TicketDto
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
            CreatedAt = ticket.CreatedAt,
            Tags = ticket.Tags,
            ViewCount = ticket.Metadata.ViewCount,
            CommentCount = ticket.Metadata.CommentCount,
            AttachmentCount = ticket.Metadata.AttachmentCount
        };
    }
}