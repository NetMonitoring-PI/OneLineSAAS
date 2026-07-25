using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OneLine.Auth.Application.Interfaces;
using OneLine.Auth.Domain.Entities;
using OneLine.Auth.Domain.Interfaces;
using OneLine.Auth.Infrastructure.Options;
using OneLine.Auth.Infrastructure.Persistence;
using OneLine.Auth.Infrastructure.Persistence.Repositories;
using OneLine.Auth.Infrastructure.Services;
using System.Text;

namespace OneLine.Auth.Infrastructure;

/// <summary>
/// Point d'entrée du module Infrastructure.
/// Enregistre tous les services concrets dans le DI container.
/// </summary>
public static class AuthInfrastructureExtensions
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Options JWT ──────────────────────────────────────
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        // ── DbContext ────────────────────────────────────────
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable(
                    "__auth_migrations", "auth")));

        // ── ASP.NET Identity ─────────────────────────────────
        services.AddIdentity<AppUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>>(options =>
        {
            // Politique de mot de passe
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;

            // Lockout après 5 tentatives échouées
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(15);

            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AuthDbContext>();

        // ── Authentication JWT ───────────────────────────────
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    // Zero → pas de tolérance sur l'expiration
                };
        });

        // ── Repositories ─────────────────────────────────────
        // Interface → Implémentation concrète
        // Principe D de SOLID : on injecte l'interface
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository,
            RefreshTokenRepository>();

        // ── Services ─────────────────────────────────────────
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
