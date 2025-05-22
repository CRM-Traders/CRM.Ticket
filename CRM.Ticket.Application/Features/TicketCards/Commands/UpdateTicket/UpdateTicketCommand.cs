using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Common.Persistence;
using CRM.Ticket.Domain.Common.Models;
using CRM.Ticket.Domain.Entities.Tickets;
using CRM.Ticket.Domain.Entities.Tickets.Enums;
using FluentValidation;

namespace CRM.Ticket.Application.Features.TicketCards.Commands.UpdateTicket;

public record UpdateTicketCommand(
    Guid Id,
    string Title,
    string Description,
    TicketPriority Priority,
    DateTimeOffset? DueDate,
    List<string>? Tags
) : IRequest<Unit>;

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ბილეთის ID აუცილებელია");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("სათაური აუცილებელია")
            .MaximumLength(200).WithMessage("სათაური არ უნდა აღემატებოდეს 200 სიმბოლოს");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("აღწერა აუცილებელია")
            .MaximumLength(4000).WithMessage("აღწერა არ უნდა აღემატებოდეს 4000 სიმბოლოს");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("პრიორიტეტი არ არის სწორი");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTimeOffset.Now)
            .When(x => x.DueDate.HasValue)
            .WithMessage("დედლაინი უნდა იყოს მომავალში");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("ტეგების რაოდენობა არ უნდა აღემატებოდეს 10-ს")
            .Must(tags => tags == null || tags.All(tag => !string.IsNullOrWhiteSpace(tag) && tag.Length <= 50))
            .WithMessage("ყველა ტეგი უნდა იყოს 50 სიმბოლოზე ნაკლები");
    }
}

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, Unit>
{
    private readonly IRepository<TicketCard> _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTicketCommandHandler(
        IRepository<TicketCard> ticketRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null)
            return Result.Failure<Unit>("ბილეთი ვერ მოიძებნა", "NotFound");

        try
        {
            ticket.Update(request.Title, request.Description, request.Priority, request.DueDate);

            if (request.Tags != null)
                ticket.UpdateTags(request.Tags);

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(Unit.Value);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Unit>(ex.Message, "BadRequest");
        }
        catch (Exception ex)
        {
            return Result.Failure<Unit>("ბილეთის განახლებისას მოხდა შეცდომა", "InternalServerError");
        }
    }
}