using CRM.Ticket.Domain.Entities.Attachments;
using CRM.Ticket.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Ticket.Persistence.Databases.Configurations;

public class TicketAttachmentConfiguration : AuditableEntityTypeConfiguration<TicketAttachment>
{
    public override void Configure(EntityTypeBuilder<TicketAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(TicketAttachment));

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(a => a.Ticket)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.TicketId);

        builder.HasIndex(a => a.UploadedBy);

        builder.HasIndex(a => a.CreatedAt);
    }
}