using CRM.Ticket.Domain.Entities.TicketMetaDatas;
using CRM.Ticket.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Ticket.Persistence.Databases.Configurations;

public class TicketMetadataConfiguration : BaseEntityTypeConfiguration<TicketMetadata>
{
    public override void Configure(EntityTypeBuilder<TicketMetadata> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(TicketMetadata));

        builder.Property(m => m.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(m => m.CommentCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(m => m.AttachmentCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(m => m.ViewCount);

        builder.HasIndex(m => m.FirstResponseAt);
    }
}