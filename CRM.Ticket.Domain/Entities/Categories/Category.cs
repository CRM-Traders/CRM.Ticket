using CRM.Ticket.Domain.Common.Entities;
using CRM.Ticket.Domain.Entities.Categories.DomainEvents;

namespace CRM.Ticket.Domain.Entities.Categories;

public class TicketCategory : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public TicketCategory(
        string name,
        string description,
        string color)
    {
        Name = name;
        Description = description;
        Color = color;
        IsActive = true;

        AddDomainEvent(new CreateCategoryEvent(
            Id, GetType().Name, name, description, color));
    }

    public void Update(string name, string description, string color)
    {
        Name = name;
        Description = description;
        Color = color;

        AddDomainEvent(new UpdateCategoryEvent(
            Id, GetType().Name, name, description, color));
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}