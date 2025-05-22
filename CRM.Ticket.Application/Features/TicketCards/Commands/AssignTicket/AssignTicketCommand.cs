using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Common.Abstractions.Users;
using CRM.Ticket.Application.Common.Persistence;
using CRM.Ticket.Domain.Common.Models;
using CRM.Ticket.Domain.Entities.Tickets;
using FluentValidation;

namespace CRM.Ticket.Application.Features.TicketCards.Commands.AssignTicket;

public sealed record AssignTicketCommand(
    Guid Id,
    Guid? AssignedToUserId
) : IRequest<Unit>;

public class AssignTicketCommandValidator : AbstractValidator<AssignTicketCommand>
{
    public AssignTicketCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ბილეთის ID აუცილებელია");
    }
}

public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, Unit>
{
    private readonly IRepository<TicketCard> _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AssignTicketCommandHandler(
        IRepository<TicketCard> ticketRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async ValueTask<Result<Unit>> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null)
            return Result.Failure<Unit>("ბილეთი ვერ მოიძებნა", "NotFound");

        if (ticket.AssignedToUserId == request.AssignedToUserId)
        {
            var message = request.AssignedToUserId.HasValue
                ? "ბილეთი უკვე ამ მომხმარებელზეა მინიჭებული"
                : "ბილეთი უკვე არ არის მინიჭებული";
            return Result.Failure<Unit>(message, "BadRequest");
        }

        try
        {
            ticket.Assign(request.AssignedToUserId, _userContext.Id);

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(Unit.Value);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<Unit>(ex.Message, "BadRequest");
        }
        catch (Exception ex)
        {
            return Result.Failure<Unit>("ბილეთის მინიჭებისას მოხდა შეცდომა", "InternalServerError");
        }
    }
}