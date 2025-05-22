using System.Reflection;
using CRM.Ticket.Application.Common.Abstractions.Users;
using CRM.Ticket.Domain.Common.Entities;
using CRM.Ticket.Domain.Entities.OutboxMessages;
using Microsoft.EntityFrameworkCore;

namespace CRM.Ticket.Persistence.Databases;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IUserContext userContext)
    : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyAuditInformation()
    {
        var userId = userContext.Id.ToString();
        var userIp = userContext.IpAddress;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreationTracking(userId, userIp);
                    break;

                case EntityState.Modified:
                    entry.Entity.SetModificationTracking(userId, userIp);

                    if (entry.Properties.Any(p => p.Metadata.Name == nameof(AuditableEntity.IsDeleted)) &&
                        entry.Property(nameof(AuditableEntity.IsDeleted)).CurrentValue is true &&
                        entry.Property(nameof(AuditableEntity.IsDeleted)).OriginalValue is false)
                    {
                        entry.Entity.SetDeletionTracking(userId, userIp);
                    }

                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.SetDeletionTracking(userId, userIp);
                    break;
            }
        }
    }
}