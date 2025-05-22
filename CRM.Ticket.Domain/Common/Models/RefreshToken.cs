namespace CRM.Ticket.Domain.Common.Models;

public sealed record RefreshToken(string Token, DateTimeOffset ValidTill);
