using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Common.Persistence;
using CRM.Ticket.Domain.Common.Models;
using CRM.Ticket.Domain.Entities.Tickets;
using FluentValidation;

namespace CRM.Ticket.Application.Features.TicketCards.Commands.DeleteTicket;

public record DeleteTicketCommand(Guid Id) : IRequest<Unit>;

public class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
{
    public DeleteTicketCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ბილეთის ID აუცილებელია");
    }
}

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, Unit>
{
    private readonly IRepository<TicketCard> _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTicketCommandHandler(
        IRepository<TicketCard> ticketRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null)
            return Result.Failure<Unit>("ბილეთი ვერ მოიძებნა", "NotFound");

        await _ticketRepository.DeleteAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}