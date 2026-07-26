using MediatR;
using OneLine.AI.Application;
using OneLine.AI.Application.UseCases.Chat;
using OneLine.AI.Application.UseCases.GetUsage;
using OneLine.AI.Infrastructure;
using OneLine.Auth.Application;
using OneLine.Auth.Infrastructure;
using OneLine.Billing.Application;
using OneLine.Billing.Application.UseCases.CreateSubscription;
using OneLine.Billing.Infrastructure;
using OneLine.Observability.Infrastructure;
using OneLine.Security.Infrastructure;
using OneLine.Security.Infrastructure.Middleware;
using OneLine.Security.Infrastructure.RateLimiting;
using OneLine.Tenants.Application;
using OneLine.Tenants.Infrastructure;
using OneLine.Tenants.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddTenantsApplication();
builder.Services.AddTenantsInfrastructure(builder.Configuration);
builder.Services.AddBillingApplication();
builder.Services.AddBillingInfrastructure(builder.Configuration);
builder.Services.AddSecurityInfrastructure(builder.Configuration);
builder.Services.AddObservabilityInfrastructure();
builder.Services.AddAIApplication();
builder.Services.AddAIInfrastructure(builder.Configuration);

// MediatR — tous les assemblies explicitement
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateSubscriptionCommand>();
    cfg.RegisterServicesFromAssemblyContaining<ChatCommand>();
    cfg.RegisterServicesFromAssemblyContaining<GetAIUsageQuery>();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseObservability();
app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
