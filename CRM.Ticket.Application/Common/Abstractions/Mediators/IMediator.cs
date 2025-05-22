using CRM.Ticket.Domain.Common.Models;

namespace CRM.Ticket.Application.Common.Abstractions.Mediators;

public interface IMediator
{
    ValueTask<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}