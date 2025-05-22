using CRM.Ticket.Domain.Common.Models;

namespace CRM.Ticket.Application.Common.Abstractions.Mediators;

public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    ValueTask<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}