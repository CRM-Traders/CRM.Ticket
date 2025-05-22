using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Common.Abstractions.Users;
using CRM.Ticket.Application.Common.Persistence;
using CRM.Ticket.Domain.Common.Models;
using CRM.Ticket.Domain.Entities.Categories;
using CRM.Ticket.Domain.Entities.Tickets;
using CRM.Ticket.Domain.Entities.Tickets.Enums;
using FluentValidation;

namespace CRM.Ticket.Application.Features.TicketCards.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketPriority Priority,
    TicketType Type,
    Guid CustomerId,
    Guid CategoryId,
    DateTimeOffset? DueDate = null,
    List<string>? Tags = null
) : IRequest<Unit>;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title Required")
            .MaximumLength(200).WithMessage("სათაური არ უნდა აღემატებოდეს 200 სიმბოლოს");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("აღწერა აუცილებელია")
            .MaximumLength(4000).WithMessage("აღწერა არ უნდა აღემატებოდეს 4000 სიმბოლოს");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("პრიორიტეტი არ არის სწორი");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("ტიპი არ არის სწორი");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("მომხმარებელი აუცილებელია");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("კატეგორია აუცილებელია");

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


public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Unit>
{
    private readonly IRepository<TicketCard> _ticketRepository;
    private readonly IRepository<TicketCategory> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CreateTicketCommandHandler(
        IRepository<TicketCard> ticketRepository,
        IRepository<TicketCategory> categoryRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _ticketRepository = ticketRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async ValueTask<Result<Unit>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            return Result.Failure<Unit>("კატეგორია ვერ მოიძებნა", "NotFound");

        if (!category.IsActive)
            return Result.Failure<Unit>("კატეგორია არააქტიურია", "BadRequest");

        var ticket = new TicketCard(
            request.Title,
            request.Description,
            request.Priority,
            request.Type,
            request.CustomerId,
            request.CategoryId,
            request.DueDate,
            request.Tags
        );

        await _ticketRepository.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}