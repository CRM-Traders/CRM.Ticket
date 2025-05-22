using CRM.Ticket.Application.Common.Abstractions.Mediators;
using CRM.Ticket.Application.Common.Persistence;
using CRM.Ticket.Application.Common.Persistence.Repositories;
using CRM.Ticket.Persistence.Databases;
using CRM.Ticket.Persistence.Repositories;
using CRM.Ticket.Persistence.Repositories.Base;
using CRM.Ticket.Persistence.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Ticket.Persistence.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddDbContext<IdentityDbContext>(options =>
        options.UseNpgsql(
                configuration.GetConnectionString("IdentityConnection"),
                b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}