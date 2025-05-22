using CRM.Ticket.Domain.Entities.TicketStatusHistories;
using CRM.Ticket.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CRM.Ticket.Persistence.Databases.Configurations;

public class TicketStatusHistoryConfiguration : BaseEntityTypeConfiguration<TicketStatusHistory>
{
    public override void Configure(EntityTypeBuilder<TicketStatusHistory> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(TicketStatusHistory));

        builder.Property(sh => sh.FromStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(sh => sh.ToStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(sh => sh.Reason)
            .HasMaxLength(500);

        builder.Property(sh => sh.ChangedAt)
            .IsRequired();

        builder.HasOne(sh => sh.Ticket)
            .WithMany(t => t.StatusHistory)
            .HasForeignKey(sh => sh.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sh => sh.TicketId);

        builder.HasIndex(sh => sh.ChangedBy);

        builder.HasIndex(sh => sh.ChangedAt);

        builder.HasIndex(sh => new { sh.TicketId, sh.ChangedAt });
    }
}