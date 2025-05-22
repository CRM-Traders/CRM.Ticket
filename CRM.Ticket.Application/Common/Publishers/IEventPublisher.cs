using CRM.Ticket.Domain.Common.Events;

namespace CRM.Ticket.Application.Common.Publishers;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}