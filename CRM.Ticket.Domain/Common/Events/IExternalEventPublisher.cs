using CRM.Ticket.Domain.Entities.OutboxMessages;

namespace CRM.Ticket.Domain.Common.Events;

public interface IExternalEventPublisher
{
    Task PublishEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default);
}