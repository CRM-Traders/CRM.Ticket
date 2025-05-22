using CRM.Ticket.Domain.Common.Entities;
using CRM.Ticket.Domain.Entities.Tickets;
using CRM.Ticket.Domain.Entities.Tickets.Enums;

namespace CRM.Ticket.Domain.Entities.TicketStatusHistories;

public class TicketStatusHistory : Entity
{
    public Guid TicketId { get; private set; }
    public TicketStatus FromStatus { get; private set; }
    public TicketStatus ToStatus { get; private set; }
    public string? Reason { get; private set; }
    public Guid ChangedBy { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }

    public TicketCard Ticket { get; private set; } = null!;

    private TicketStatusHistory() {}

    public TicketStatusHistory(
        Guid ticketId,
        TicketStatus fromStatus,
        TicketStatus toStatus,
        Guid changedBy,
        string? reason = null)
    {
        TicketId = ticketId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ChangedBy = changedBy;
        Reason = reason;
        ChangedAt = DateTimeOffset.UtcNow;
    }
}