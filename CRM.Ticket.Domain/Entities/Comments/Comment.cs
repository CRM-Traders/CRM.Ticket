using CRM.Ticket.Domain.Common.Entities;
using CRM.Ticket.Domain.Entities.Tickets;

namespace CRM.Ticket.Domain.Entities.Comments;

public class Comment : AuditableEntity
{
    public Guid TicketId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Guid AuthorId { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTimeOffset? EditedAt { get; private set; }

    public Comment(
        Guid ticketId,
        string content,
        Guid authorId)
    {
        TicketId = ticketId;
        Content = content;
        AuthorId = authorId;
        IsEdited = false;
    }

    public void Edit(string content)
    {
        Content = content;
        IsEdited = true;
        EditedAt = DateTimeOffset.UtcNow;
    }
}
