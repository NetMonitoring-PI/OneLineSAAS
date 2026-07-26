using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneLine.AI.Application.Interfaces;
using OneLine.AI.Domain.Interfaces;
using OneLine.AI.Infrastructure.Options;
using OneLine.AI.Infrastructure.Persistence;
using OneLine.AI.Infrastructure.Persistence.Repositories;
using OneLine.AI.Infrastructure.Services;

namespace OneLine.AI.Infrastructure;

public static class AIInfrastructureExtensions
{
    public static IServiceCollection AddAIInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Options AI
        var aiSection = configuration.GetSection(AIOptions.SectionName);
        services.Configure<AIOptions>(opts =>
        {
            opts.ApiKey = aiSection["ApiKey"] ?? string.Empty;
            opts.Model = aiSection["Model"] ?? "gpt-4o-mini";
            opts.MaxTokens = int.Parse(aiSection["MaxTokens"] ?? "2000");
            opts.Temperature = float.Parse(
                aiSection["Temperature"] ?? "0.7",
                System.Globalization.CultureInfo.InvariantCulture);
        });

        // DbContext
        services.AddDbContext<AIDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IAIUsageRepository, AIUsageRepository>();
        services.AddScoped<IAIConversationRepository, AIConversationRepository>();

        // UnitOfWork
        services.AddScoped<IUnitOfWork, AIUnitOfWork>();

        // LLM Service â€” Mock si pas de cle API, OpenAI sinon
        var apiKey = aiSection["ApiKey"] ?? string.Empty;
        if (string.IsNullOrEmpty(apiKey) || apiKey == "sk-YOUR_KEY_HERE")
        {
            services.AddScoped<ILLMService, MockLLMService>();
        }
        else
        {
            services.AddScoped<ILLMService, OpenAILLMService>();
        }

        return services;
    }
}
