using CRM.Ticket.Domain.Common.Entities;

namespace CRM.Ticket.Domain.Entities.TicketMetaDatas;

public class TicketMetadata : Entity
{
    public int ViewCount { get; private set; }
    public DateTimeOffset? FirstResponseAt { get; private set; }
    public TimeSpan? AverageResponseTime { get; private set; }
    public int CommentCount { get; private set; }
    public int AttachmentCount { get; private set; }

    private TicketMetadata() { }

    public static TicketMetadata Create()
    {
        return new TicketMetadata();
    }

    public void IncrementViewCount() => ViewCount++;
    public void SetFirstResponse(DateTimeOffset responseTime) => FirstResponseAt = responseTime;
    public void UpdateCommentCount(int count) => CommentCount = count;
    public void UpdateAttachmentCount(int count) => AttachmentCount = count;
}