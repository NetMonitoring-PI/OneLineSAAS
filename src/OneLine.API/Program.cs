using OneLine.Auth.Application;
using OneLine.Auth.Infrastructure;
using OneLine.Tenants.Application;
using OneLine.Tenants.Infrastructure;
using OneLine.Tenants.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddTenantsApplication();
builder.Services.AddTenantsInfrastructure(builder.Configuration);

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
app.MapControllers();

app.Run();
