using OneLine.Auth.Application;
using OneLine.Auth.Infrastructure;
using OneLine.Billing.Application;
using OneLine.Billing.Application.UseCases.CreateSubscription;
using OneLine.Billing.Infrastructure;
using OneLine.Billing.Infrastructure.Middleware;
using OneLine.Tenants.Application;
using OneLine.Tenants.Infrastructure;
using OneLine.Tenants.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Modules ──────────────────────────────────────────────────
builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddTenantsApplication();
builder.Services.AddTenantsInfrastructure(builder.Configuration);
builder.Services.AddBillingApplication();
builder.Services.AddBillingInfrastructure(builder.Configuration);

// ── MediatR explicite pour Billing ───────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CreateSubscriptionCommand).Assembly));

// ── API ──────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SubscriptionMiddleware>();
app.MapControllers();

app.Run();
