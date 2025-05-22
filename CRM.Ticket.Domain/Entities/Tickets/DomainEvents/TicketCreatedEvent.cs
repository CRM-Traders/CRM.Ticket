using CRM.Ticket.Domain.Common.Events;
using CRM.Ticket.Domain.Entities.Tickets.Enums;

namespace CRM.Ticket.Domain.Entities.Tickets.DomainEvents;

public class TicketCreatedEvent : DomainEvent
{
    public string Title { get; }
    public string Description { get; }
    public TicketPriority Priority { get; }
    public TicketType Type { get; }
    public Guid CustomerId { get; }
    public Guid CategoryId { get; }

    public TicketCreatedEvent(
        Guid aggregateId,
        string aggregateType,
        string title,
        string description,
        TicketPriority priority,
        TicketType type,
        Guid customerId,
        Guid categoryId) : base(aggregateId, aggregateType)
    {
        Title = title;
        Description = description;
        Priority = priority;
        Type = type;
        CustomerId = customerId;
        CategoryId = categoryId;
    }
}