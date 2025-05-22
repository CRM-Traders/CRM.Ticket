using CRM.Ticket.Domain.Common.Events;

namespace CRM.Ticket.Domain.Entities.Tickets.DomainEvents;

public class TicketAssignedEvent : DomainEvent
{
    public Guid? PreviousAssigneeId { get; }
    public Guid? NewAssigneeId { get; }
    public Guid AssignedBy { get; }

    public TicketAssignedEvent(
        Guid aggregateId,
        string aggregateType,
        Guid? previousAssigneeId,
        Guid? newAssigneeId,
        Guid assignedBy) : base(aggregateId, aggregateType)
    {
        PreviousAssigneeId = previousAssigneeId;
        NewAssigneeId = newAssigneeId;
        AssignedBy = assignedBy;
    }
}
