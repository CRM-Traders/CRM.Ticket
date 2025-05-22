using CRM.Ticket.Domain.Common.Entities;
using CRM.Ticket.Domain.Entities.Tickets;

namespace CRM.Ticket.Domain.Entities.Comments;

public class TicketComment : AuditableEntity
{
    public Guid TicketId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Guid AuthorId { get; private set; }
    public bool IsInternal { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTimeOffset? EditedAt { get; private set; }
    public Guid? ParentCommentId { get; private set; }

    public TicketCard Ticket { get; private set; } = null!;
    public TicketComment? ParentComment { get; private set; }
    private readonly List<TicketComment> _replies = new();
    public IReadOnlyCollection<TicketComment> Replies => _replies.AsReadOnly();

    private TicketComment() { }

    public TicketComment(
        Guid ticketId,
        string content,
        Guid authorId,
        bool isInternal = false,
        Guid? parentCommentId = null)
    {
        TicketId = ticketId;
        Content = content;
        AuthorId = authorId;
        IsInternal = isInternal;
        ParentCommentId = parentCommentId;
        IsEdited = false;
    }

    public void Edit(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty", nameof(content));

        Content = content;
        IsEdited = true;
        EditedAt = DateTimeOffset.UtcNow;
    }

    public void AddReply(TicketComment reply)
    {
        if (reply.ParentCommentId != Id)
            throw new InvalidOperationException("Reply must have this comment as parent");

        _replies.Add(reply);
    }
}
