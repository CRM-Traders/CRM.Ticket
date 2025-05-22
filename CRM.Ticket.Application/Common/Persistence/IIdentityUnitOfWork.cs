namespace CRM.Ticket.Application.Common.Persistence;

public interface IIdentityUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}