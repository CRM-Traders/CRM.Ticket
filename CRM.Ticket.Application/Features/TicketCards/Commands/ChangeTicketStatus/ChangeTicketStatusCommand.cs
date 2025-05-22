using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Common.Abstractions.Users;
using CRM.Ticket.Application.Common.Persistence;
using CRM.Ticket.Domain.Common.Models;
using CRM.Ticket.Domain.Entities.Tickets;
using CRM.Ticket.Domain.Entities.Tickets.Enums;
using FluentValidation;

namespace CRM.Ticket.Application.Features.TicketCards.Commands.ChangeTicketStatus;

public record ChangeTicketStatusCommand(
    Guid Id,
    TicketStatus Status,
    string? Reason = null
) : IRequest<Unit>;

public class ChangeTicketStatusCommandValidator : AbstractValidator<ChangeTicketStatusCommand>
{
    public ChangeTicketStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ბილეთის ID აუცილებელია");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("სტატუსი არ არის სწორი");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("მიზეზი არ უნდა აღემატებოდეს 500 სიმბოლოს")
            .When(x => !string.IsNullOrEmpty(x.Reason));

        RuleFor(x => x.Reason)
            .NotEmpty()
            .When(x => x.Status == TicketStatus.OnHold || x.Status == TicketStatus.Closed)
            .WithMessage("ამ სტატუსისთვის მიზეზის მითითება სავალდებულოა");
    }
}

public class ChangeTicketStatusCommandHandler : IRequestHandler<ChangeTicketStatusCommand, Unit>
{
    private readonly IRepository<TicketCard> _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ChangeTicketStatusCommandHandler(
        IRepository<TicketCard> ticketRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async ValueTask<Result<Unit>> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null)
            return Result.Failure<Unit>("ბილეთი ვერ მოიძებნა", "NotFound");

        var validationResult = ValidateStatusTransition(ticket.Status, request.Status);
        if (!validationResult.IsSuccess)
            return validationResult;

        try
        {
            ticket.ChangeStatus(request.Status, _userContext.Id, request.Reason);

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
            return Result.Failure<Unit>("სტატუსის შეცვლისას მოხდა შეცდომა", "InternalServerError");
        }
    }

    private static Result<Unit> ValidateStatusTransition(TicketStatus currentStatus, TicketStatus newStatus)
    {
        var allowedTransitions = new Dictionary<TicketStatus, TicketStatus[]>
        {
            { TicketStatus.Open, new[] { TicketStatus.InProgress, TicketStatus.OnHold, TicketStatus.Resolved, TicketStatus.Closed } },
            { TicketStatus.InProgress, new[] { TicketStatus.OnHold, TicketStatus.Resolved, TicketStatus.Closed, TicketStatus.Open } },
            { TicketStatus.OnHold, new[] { TicketStatus.InProgress, TicketStatus.Open, TicketStatus.Closed } },
            { TicketStatus.Resolved, new[] { TicketStatus.Closed, TicketStatus.Reopened } },
            { TicketStatus.Closed, new[] { TicketStatus.Reopened } },
            { TicketStatus.Reopened, new[] { TicketStatus.InProgress, TicketStatus.OnHold, TicketStatus.Resolved, TicketStatus.Closed } }
        };

        if (currentStatus == newStatus)
            return Result.Failure<Unit>("ბილეთი უკვე ამ სტატუსშია", "BadRequest");

        if (allowedTransitions.TryGetValue(currentStatus, out var allowed) && !allowed.Contains(newStatus))
            return Result.Failure<Unit>($"სტატუსის შეცვლა {currentStatus}-დან {newStatus}-ზე არ არის ნებადართული", "BadRequest");

        return Result.Success(Unit.Value);
    }
}