using System.Net.Sockets;
using CRM.Ticket.Domain.Common.Entities;
using CRM.Ticket.Domain.Entities.Tickets.Enums;

namespace CRM.Ticket.Domain.Entities.Tickets;

public class Ticket : AggregateRoot
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
    public List<string> Tags { get; private set; } = new();
    public TicketMetadata Metadata { get; private set; }

    private readonly List<TicketComment> _comments = new();
    private readonly List<TicketAttachment> _attachments = new();
    private readonly List<TicketStatusHistory> _statusHistory = new();

    public IReadOnlyCollection<TicketComment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<TicketAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<TicketStatusHistory> StatusHistory => _statusHistory.AsReadOnly();
}
