using CRM.Ticket.Domain.Common.Entities;
using CRM.Ticket.Domain.Entities.Attachments;
using CRM.Ticket.Domain.Entities.Categories;
using CRM.Ticket.Domain.Entities.Comments;
using CRM.Ticket.Domain.Entities.TicketMetaDatas;
using CRM.Ticket.Domain.Entities.Tickets.DomainEvents;
using CRM.Ticket.Domain.Entities.Tickets.Enums;
using CRM.Ticket.Domain.Entities.TicketStatusHistories;

namespace CRM.Ticket.Domain.Entities.Tickets;

public class TicketCard : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public TicketType Type { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? ResolutionNotes { get; private set; }
    private string _tags = string.Empty;
    public List<string> Tags
    {
        get => string.IsNullOrEmpty(_tags) ? new() : _tags.Split(',').ToList();
        private set => _tags = string.Join(",", value);
    }

    public TicketCategory Category { get; private set; } = null!;
    public TicketMetadata Metadata { get; private set; } = null!;

    private readonly List<TicketComment> _comments = new();
    private readonly List<TicketAttachment> _attachments = new();
    private readonly List<TicketStatusHistory> _statusHistory = new();

    public IReadOnlyCollection<TicketComment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<TicketAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<TicketStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    private TicketCard() {} 

    public TicketCard(
        string title,
        string description,
        TicketPriority priority,
        TicketType type,
        Guid customerId,
        Guid categoryId,
        DateTimeOffset? dueDate = null,
        List<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Title = title;
        Description = description;
        Priority = priority;
        Type = type;
        CustomerId = customerId;
        CategoryId = categoryId;
        DueDate = dueDate;
        Status = TicketStatus.Open;
        Tags = tags ?? new List<string>();

        Metadata = TicketMetadata.Create();

        AddDomainEvent(new TicketCreatedEvent(
            Id, GetType().Name, title, description, priority, type, customerId, categoryId));
    }

    public void Update(string title, string description, TicketPriority priority, DateTimeOffset? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
    }

    public void ChangeStatus(TicketStatus newStatus, Guid changedBy, string? reason = null)
    {
        if (Status == newStatus) return;

        var oldStatus = Status;
        Status = newStatus;

        var statusChange = new TicketStatusHistory(Id, oldStatus, newStatus, changedBy, reason);
        _statusHistory.Add(statusChange);

        switch (newStatus)
        {
            case TicketStatus.Resolved:
                ResolvedAt = DateTimeOffset.UtcNow;
                break;
            case TicketStatus.Closed:
                ClosedAt = DateTimeOffset.UtcNow;
                if (!ResolvedAt.HasValue)
                    ResolvedAt = DateTimeOffset.UtcNow;
                break;
            case TicketStatus.Reopened:
                ResolvedAt = null;
                ClosedAt = null;
                ResolutionNotes = null;
                break;
        }

        AddDomainEvent(new TicketStatusChangedEvent(Id, GetType().Name, oldStatus, newStatus, reason, changedBy));
    }

    public void Assign(Guid? userId, Guid assignedBy)
    {
        var previousAssigneeId = AssignedToUserId;
        AssignedToUserId = userId;

        AddDomainEvent(new TicketAssignedEvent(Id, GetType().Name, previousAssigneeId, userId, assignedBy));
    }

    public void Resolve(string resolutionNotes, Guid resolvedBy)
    {
        if (Status == TicketStatus.Closed)
            throw new InvalidOperationException("Cannot resolve a closed ticket");

        ResolutionNotes = resolutionNotes;
        ChangeStatus(TicketStatus.Resolved, resolvedBy, "Ticket resolved");
    }

    public void Close(Guid closedBy, string? reason = null)
    {
        if (Status == TicketStatus.Closed)
            throw new InvalidOperationException("Ticket is already closed");

        ChangeStatus(TicketStatus.Closed, closedBy, reason ?? "Ticket closed");
    }

    public void Reopen(Guid reopenedBy, string? reason = null)
    {
        if (Status != TicketStatus.Closed && Status != TicketStatus.Resolved)
            throw new InvalidOperationException("Can only reopen closed or resolved tickets");

        ChangeStatus(TicketStatus.Reopened, reopenedBy, reason ?? "Ticket reopened");
    }

    public TicketComment AddComment(string content, Guid authorId, bool isInternal = false, Guid? parentCommentId = null)
    {
        var comment = new TicketComment(Id, content, authorId, isInternal, parentCommentId);
        _comments.Add(comment);

        Metadata.UpdateCommentCount(_comments.Count);

        return comment;
    }

    public TicketAttachment AddAttachment(
        string fileName,
        Guid uploadedBy)
    {
        var attachment = new TicketAttachment(Id, fileName, uploadedBy);
        _attachments.Add(attachment);

        Metadata.UpdateAttachmentCount(_attachments.Count);

        return attachment;
    }

    public void RemoveAttachment(Guid attachmentId)
    {
        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment != null)
        {
            _attachments.Remove(attachment);
            Metadata.UpdateAttachmentCount(_attachments.Count);
        }
    }

    public void UpdateTags(List<string> tags)
    {
        Tags = tags ?? new List<string>();
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return;

        var currentTags = Tags;
        if (!currentTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            currentTags.Add(tag);
            Tags = currentTags;
        }
    }

    public void RemoveTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return;

        var currentTags = Tags;
        currentTags.RemoveAll(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));
        Tags = currentTags;
    }

    public void IncrementViewCount()
    {
        Metadata.IncrementViewCount();
    }

    public void SetFirstResponse()
    {
        if (!Metadata.FirstResponseAt.HasValue)
        {
            Metadata.SetFirstResponse(DateTimeOffset.UtcNow);
        }
    }
}
