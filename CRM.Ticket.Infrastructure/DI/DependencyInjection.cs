using System.Security.Cryptography;
using CRM.Ticket.Application.Common.Abstractions.Users;
using CRM.Ticket.Application.Common.Publishers;
using CRM.Ticket.Application.Common.Services.Grids;
using CRM.Ticket.Application.Common.Services.Outbox;
using CRM.Ticket.Application.Common.Services.Synchronizer;
using CRM.Ticket.Domain.Common.Options.Auth;
using CRM.Ticket.Infrastructure.Contexts;
using CRM.Ticket.Infrastructure.Publishers;
using CRM.Ticket.Infrastructure.Services.Grids;
using CRM.Ticket.Infrastructure.Services.Outbox;
using CRM.Ticket.Infrastructure.Services.Synchronizer;
using CRM.Ticket.Infrastructure.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Ticket.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingeltonServices();
        services.AddScopedServices();
        services.AddEventHandlers();

        services.AddCompression();
        services.ConfigureCors();

        services.AddOptions(configuration);

        services.AddAsymmetricAuthentication(configuration);

        services.AddHostedService<OutboxProcessorWorker>();

        return services;
    }

    private static void ConfigureCors(this IServiceCollection services)
    {
        // TODO Restrict In Future Base On Origins Options
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    private static void AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;
        services.AddSingleton(jwtOptions);
    }

    private static void AddSingeltonServices(this IServiceCollection services)
    {
    }

    private static void AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();

        services.AddScoped<IPermissionSynchronizer, PermissionSynchronizer>();

        services.AddScoped<IGridService, GridService>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
    }

    private static void AddCompression(this IServiceCollection services)
    {
        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        services.AddResponseCompression(options =>
        {
            options.Providers.Add<GzipCompressionProvider>();
            options.EnableForHttps = true;
        });
    }

    private static void AddAsymmetricAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;

        byte[] publicKeyBytes = Convert.FromBase64String(jwtOptions.PublicKey);

        RSA rsaPublicKey = RSA.Create();
        rsaPublicKey.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var issuerSigningKey = new RsaSecurityKey(rsaPublicKey);


        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = issuerSigningKey,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
    }
}