namespace CRM.Ticket.Application.Common.Services.Outbox;

public interface IOutboxProcessor
{
    Task ProcessPendingMessagesAsync(CancellationToken cancellationToken = default);
}
