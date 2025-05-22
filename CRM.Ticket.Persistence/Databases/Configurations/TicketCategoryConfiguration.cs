using CRM.Ticket.Domain.Entities.Categories;
using CRM.Ticket.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Ticket.Persistence.Databases.Configurations;

public class TicketCategoryConfiguration : AuditableEntityTypeConfiguration<TicketCategory>
{
    public override void Configure(EntityTypeBuilder<TicketCategory> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(TicketCategory));

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasIndex(c => c.IsActive);
    }
}
