using CRM.Ticket.Api.Controllers.Base;
using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Features.TicketCards.Commands.AssignTicket;
using CRM.Ticket.Application.Features.TicketCards.Commands.ChangeTicketStatus;
using CRM.Ticket.Application.Features.TicketCards.Commands.CreateTicket;
using CRM.Ticket.Application.Features.TicketCards.Commands.DeleteTicket;
using CRM.Ticket.Application.Features.TicketCards.Commands.UpdateTicket;
using CRM.Ticket.Application.Features.TicketCards.Queries.GetTicket;
using CRM.Ticket.Application.Features.TicketCards.Queries.GetTickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Ticket.Api.Controllers;

public class TicketsController(IMediator _sender) : BaseController(_sender)
{
    [HttpGet]
    [Authorize]
    //[Permission(1, "View Tickets", "Tickets", ActionType.V, "Admin,Manager,User")]
    public async Task<IResult> GetTickets([FromQuery] GetTicketsQuery query, CancellationToken cancellationToken)
    {
        return await SendAsync(query, cancellationToken);
    }

    [HttpGet("details")]
    [Authorize]
    //[Permission(2, "ნახვა", "Tickets", ActionType.V, "Admin,Manager,User")]
    public async Task<IResult> GetTicketById([FromQuery] GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        return await SendAsync(request, cancellationToken);
    }

    [HttpPost]
    [Authorize]
    //[Permission(4, "შექმნა", "Tickets", ActionType.C, "Admin,Manager,User")]
    public async Task<IResult> CreateTicket([FromBody] CreateTicketCommand command, CancellationToken cancellationToken)
    {
        return await SendAsync(command, cancellationToken);
    }

    [HttpPut]
    [Authorize]
    //[Permission(5, "რედაქტირება", "Tickets", ActionType.E, "Admin,Manager")]
    public async Task<IResult> UpdateTicket([FromBody] UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        return await SendAsync(request, cancellationToken);
    }

    [HttpPatch()]
    [Authorize]
    //[Permission(6, "სტატუსის შეცვლა", "Tickets", ActionType.E, "Admin,Manager")]
    public async Task<IResult> ChangeTicketStatus([FromBody] ChangeTicketStatusCommand request, CancellationToken cancellationToken)
    {
        return await SendAsync(request, cancellationToken);
    }

    [HttpPatch("assign")]
    [Authorize]
    //[Permission(7, "მინიჭება", "Tickets", ActionType.E, "Admin,Manager")]
    public async Task<IResult> AssignTicket([FromBody] AssignTicketCommand request, CancellationToken cancellationToken)
    {
        return await SendAsync(request, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    //[Permission(8, "წაშლა", "Tickets", ActionType.D, "Admin")]
    public async Task<IResult> DeleteTicket([FromQuery] DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        return await SendAsync(request, cancellationToken);
    }
}
