using CRM.Ticket.Application.Common.Abstractions.Specifications;
using CRM.Ticket.Domain.Entities.OutboxMessages;

namespace CRM.Ticket.Application.Common.Specifications.Events;

public class OutboxMessageByDomainEventIdSpecification : BaseSpecification<OutboxMessage>
{
    public OutboxMessageByDomainEventIdSpecification(Guid domainEventId)
        : base(message => message.Id == domainEventId)
    {
    }
}