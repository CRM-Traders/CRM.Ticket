using CRM.Ticket.Domain.Common.Models;

namespace CRM.Ticket.Application.Common.Abstractions.Mediators;

public delegate ValueTask<Result<TResponse>> RequestHandlerDelegate<TResponse>();
