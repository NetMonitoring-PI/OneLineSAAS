using Microsoft.EntityFrameworkCore;
using OneLine.Auth.Application;
using OneLine.Auth.Infrastructure;
using OneLine.Auth.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ── Modules ──────────────────────────────────────────────
builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);

// ── API ──────────────────────────────────────────────────
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();