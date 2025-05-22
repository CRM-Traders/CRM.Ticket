using CRM.Ticket.Domain.Common.Events;
using CRM.Ticket.Domain.Entities.Tickets.Enums;

namespace CRM.Ticket.Domain.Entities.Tickets.DomainEvents;

public class TicketStatusChangedEvent : DomainEvent
{
    public TicketStatus OldStatus { get; }
    public TicketStatus NewStatus { get; }
    public string? Reason { get; }
    public Guid? ChangedBy { get; }

    public TicketStatusChangedEvent(
        Guid aggregateId,
        string aggregateType,
        TicketStatus oldStatus,
        TicketStatus newStatus,
        string? reason = null,
        Guid? changedBy = null) : base(aggregateId, aggregateType)
    {
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
        ChangedBy = changedBy;
    }
}