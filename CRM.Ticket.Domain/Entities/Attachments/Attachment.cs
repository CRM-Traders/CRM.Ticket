using CRM.Ticket.Domain.Common.Entities;
using CRM.Ticket.Domain.Entities.Tickets;

namespace CRM.Ticket.Domain.Entities.Attachments;

public class TicketAttachment : AuditableEntity
{
    public Guid TicketId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public Guid UploadedBy { get; private set; }

    public TicketCard Ticket { get; private set; } = null!;

    private TicketAttachment() { } 

    public TicketAttachment(
        Guid ticketId,
        string fileName,
        Guid uploadedBy)
    {
        TicketId = ticketId;
        FileName = fileName;
        UploadedBy = uploadedBy;
    }
}