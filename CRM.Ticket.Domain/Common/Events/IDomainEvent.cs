﻿namespace CRM.Ticket.Domain.Common.Events;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
    Guid AggregateId { get; }
    string AggregateType { get; }
    ProcessingStrategy ProcessingStrategy { get; }
}