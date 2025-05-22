using CRM.Ticket.Domain.Common.Events;

namespace CRM.Ticket.Domain.Entities.Categories.DomainEvents;

public class CreateCategoryEvent : DomainEvent
{
    public string Name { get;  } = string.Empty;
    public string Description { get; } = string.Empty;
    public string Color { get;  } = string.Empty;
    public bool IsActive { get; }

    public CreateCategoryEvent(
        Guid aggregateId,
        string aggregateType,
        string name,
        string description,
        string color,
        bool isActive = true) : base(aggregateId, aggregateType)
    {
        Name = name;
        Description = description;
        Color = color;
        IsActive = isActive;
    }
}
