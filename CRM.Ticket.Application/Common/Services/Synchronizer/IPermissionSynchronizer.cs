namespace CRM.Ticket.Application.Common.Services.Synchronizer;

public interface IPermissionSynchronizer
{
    Task SynchronizePermissionsAsync(CancellationToken cancellationToken = default);
}