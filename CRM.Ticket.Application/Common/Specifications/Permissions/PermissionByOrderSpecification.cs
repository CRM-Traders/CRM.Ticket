using CRM.Ticket.Application.Common.Abstractions.Specifications;
using CRM.Ticket.Domain.Entities.Permissions;

namespace CRM.Ticket.Application.Common.Specifications.Permissions;

public class PermissionByOrderSpecification(int order) : BaseSpecification<Permission>(p => p.Order == order);