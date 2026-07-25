using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Billing.Infrastructure.Options;
using OneLine.Billing.Infrastructure.Persistence;
using OneLine.Billing.Infrastructure.Persistence.Repositories;
using OneLine.Billing.Infrastructure.Services;
using OneLine.Billing.Infrastructure.Webhooks;

namespace OneLine.Billing.Infrastructure;

public static class BillingInfrastructureExtensions
{
    public static IServiceCollection AddBillingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var stripeSection = configuration.GetSection(StripeOptions.SectionName);
        services.Configure<StripeOptions>(opts =>
        {
            opts.SecretKey = stripeSection["SecretKey"] ?? string.Empty;
            opts.PublishableKey = stripeSection["PublishableKey"] ?? string.Empty;
            opts.WebhookSecret = stripeSection["WebhookSecret"] ?? string.Empty;
        });

        services.AddDbContext<BillingDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IUnitOfWork, BillingUnitOfWork>();
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<StripeWebhookHandler>();

        return services;
    }
}
