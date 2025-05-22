using CRM.Ticket.Domain.Entities.Comments;
using CRM.Ticket.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CRM.Ticket.Persistence.Databases.Configurations;

public class TicketCommentConfiguration : AuditableEntityTypeConfiguration<TicketComment>
{
    public override void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(TicketComment));

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(c => c.IsInternal)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.IsEdited)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(c => c.Ticket)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.TicketId);

        builder.HasIndex(c => c.AuthorId);

        builder.HasIndex(c => c.ParentCommentId);

        builder.HasIndex(c => c.CreatedAt);

        builder.HasIndex(c => new { c.TicketId, c.IsInternal });
    }
}