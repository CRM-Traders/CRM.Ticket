using CRM.Ticket.Domain.Common.Entities;

namespace CRM.Ticket.Domain.Entities.Attachments;

public class Attachment : AuditableEntity
{
    public Guid FileId { get; private set; }
    public Guid TicketId { get; private set; }

    public Attachment(Guid fileId, Guid ticketId)
    {
        FileId = fileId;
        TicketId = ticketId;
    }
}
