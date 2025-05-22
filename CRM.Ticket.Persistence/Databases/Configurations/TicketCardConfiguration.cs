using CRM.Ticket.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CRM.Ticket.Domain.Entities.Tickets;

namespace CRM.Ticket.Persistence.Databases.Configurations;

public class TicketConfiguration : AuditableEntityTypeConfiguration<TicketCard>
{
    public override void Configure(EntityTypeBuilder<TicketCard> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(TicketCard));

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.ResolutionNotes)
            .HasMaxLength(2000);

        builder.Property(t => t.Tags)
            .HasColumnName("Tags")
            .HasMaxLength(1000)
            .HasConversion(
                v => string.Join(",", v),
                v => string.IsNullOrEmpty(v) ? new List<string>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );

        builder.HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Metadata)
            .WithOne()
            .HasForeignKey<TicketCard>(t => t.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Comments)
            .WithOne(c => c.Ticket)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Attachments)
            .WithOne(a => a.Ticket)
            .HasForeignKey(a => a.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.StatusHistory)
            .WithOne(sh => sh.Ticket)
            .HasForeignKey(sh => sh.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Status);

        builder.HasIndex(t => t.Priority);

        builder.HasIndex(t => t.CustomerId);

        builder.HasIndex(t => t.AssignedToUserId);

        builder.HasIndex(t => t.CategoryId);

        builder.HasIndex(t => t.CreatedAt);

        builder.HasIndex(t => new { t.Status, t.Priority });

        builder.HasIndex(t => new { t.CustomerId, t.Status });
    }
}