# ============================================================
# Script Module AI - OneLine.AI
# Executer depuis : C:\Users\DELL\Projects\OneLine.SaasKit
# ============================================================

Write-Host "=== Module AI ===" -ForegroundColor Cyan

# ── ETAPE 1 : Creer les projets ──────────────────────────────
Write-Host "`n[1/6] Creation des projets..." -ForegroundColor Yellow

dotnet new classlib -n OneLine.AI.Domain -o src\Modules\AI\OneLine.AI.Domain --force
dotnet new classlib -n OneLine.AI.Application -o src\Modules\AI\OneLine.AI.Application --force
dotnet new classlib -n OneLine.AI.Infrastructure -o src\Modules\AI\OneLine.AI.Infrastructure --force

dotnet sln add src\Modules\AI\OneLine.AI.Domain\OneLine.AI.Domain.csproj
dotnet sln add src\Modules\AI\OneLine.AI.Application\OneLine.AI.Application.csproj
dotnet sln add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj

Remove-Item -Force src\Modules\AI\OneLine.AI.Domain\Class1.cs -ErrorAction SilentlyContinue
Remove-Item -Force src\Modules\AI\OneLine.AI.Application\Class1.cs -ErrorAction SilentlyContinue
Remove-Item -Force src\Modules\AI\OneLine.AI.Infrastructure\Class1.cs -ErrorAction SilentlyContinue

Write-Host "Projets crees" -ForegroundColor Green

# ── ETAPE 2 : References et packages ─────────────────────────
Write-Host "`n[2/6] References et packages..." -ForegroundColor Yellow

# Domain
dotnet add src\Modules\AI\OneLine.AI.Domain\OneLine.AI.Domain.csproj `
  reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj
dotnet add src\Modules\AI\OneLine.AI.Domain\OneLine.AI.Domain.csproj `
  package MediatR --version 12.2.0

# Application
dotnet add src\Modules\AI\OneLine.AI.Application\OneLine.AI.Application.csproj `
  reference src\Modules\AI\OneLine.AI.Domain\OneLine.AI.Domain.csproj
dotnet add src\Modules\AI\OneLine.AI.Application\OneLine.AI.Application.csproj `
  reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj
dotnet add src\Modules\AI\OneLine.AI.Application\OneLine.AI.Application.csproj `
  package MediatR --version 12.2.0
dotnet add src\Modules\AI\OneLine.AI.Application\OneLine.AI.Application.csproj `
  package FluentValidation --version 11.9.0
dotnet add src\Modules\AI\OneLine.AI.Application\OneLine.AI.Application.csproj `
  package FluentValidation.DependencyInjectionExtensions --version 11.9.0
dotnet add src\Modules\AI\OneLine.AI.Application\OneLine.AI.Application.csproj `
  package Microsoft.Extensions.DependencyInjection.Abstractions --version 9.0.0

# Infrastructure
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  reference src\Modules\AI\OneLine.AI.Application\OneLine.AI.Application.csproj
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  package Microsoft.AspNetCore.Http --version 2.2.2
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  package Microsoft.Extensions.DependencyInjection.Abstractions --version 9.0.0
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  package Microsoft.Extensions.Caching.Memory --version 9.0.0
dotnet add src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  package Azure.AI.OpenAI --version 2.1.0

# API
dotnet add src\OneLine.API\OneLine.API.csproj `
  reference src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj

# Migrations
dotnet add tools\OneLine.Migrations\OneLine.Migrations.csproj `
  reference src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj

Write-Host "References OK" -ForegroundColor Green

# ── ETAPE 3 : Creer les dossiers ─────────────────────────────
Write-Host "`n[3/6] Creation des dossiers..." -ForegroundColor Yellow

$dirs = @(
    "src\Modules\AI\OneLine.AI.Domain\Entities",
    "src\Modules\AI\OneLine.AI.Domain\Interfaces",
    "src\Modules\AI\OneLine.AI.Domain\Enums",
    "src\Modules\AI\OneLine.AI.Domain\Errors",
    "src\Modules\AI\OneLine.AI.Application\DTOs",
    "src\Modules\AI\OneLine.AI.Application\Interfaces",
    "src\Modules\AI\OneLine.AI.Application\UseCases\Chat",
    "src\Modules\AI\OneLine.AI.Application\UseCases\GetUsage",
    "src\Modules\AI\OneLine.AI.Infrastructure\Persistence\Repositories",
    "src\Modules\AI\OneLine.AI.Infrastructure\Services",
    "src\Modules\AI\OneLine.AI.Infrastructure\Middleware",
    "src\Modules\AI\OneLine.AI.Infrastructure\Options"
)
foreach ($dir in $dirs) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

Write-Host "Dossiers crees" -ForegroundColor Green

# ── ETAPE 4 : Creer les fichiers ─────────────────────────────
Write-Host "`n[4/6] Creation des fichiers..." -ForegroundColor Yellow

# ── DOMAIN ──────────────────────────────────────────────────

Set-Content -Path "src\Modules\AI\OneLine.AI.Domain\Enums\AIEnums.cs" -Encoding UTF8 -Value @'
namespace OneLine.AI.Domain.Enums;

public enum AIProvider
{
    OpenAI = 0,
    Mistral = 1,
    Groq = 2,
    Ollama = 3
}

public enum MessageRole
{
    System = 0,
    User = 1,
    Assistant = 2
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Domain\Entities\AIUsage.cs" -Encoding UTF8 -Value @'
using OneLine.Shared.Domain.Primitives;

namespace OneLine.AI.Domain.Entities;

/// <summary>
/// Enregistre la consommation de tokens IA par tenant.
///
/// Pourquoi tracker par tenant ?
///   - Facturation a l usage (Stripe Metered Billing)
///   - Respect des quotas par plan (Free = 10k tokens/mois)
///   - Tableau de bord d utilisation pour chaque client
///
/// Pattern : Audit Entity
/// </summary>
public sealed class AIUsage : BaseEntity
{
    public Guid TenantId { get; private set; }
    public int PromptTokens { get; private set; }
    public int CompletionTokens { get; private set; }
    public int TotalTokens { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string Provider { get; private set; } = string.Empty;
    public decimal EstimatedCostUsd { get; private set; }
    public string? ConversationId { get; private set; }

    private AIUsage() { }

    public static AIUsage Create(
        Guid tenantId,
        int promptTokens,
        int completionTokens,
        string model,
        string provider,
        string? conversationId = null)
    {
        var totalTokens = promptTokens + completionTokens;

        // Estimation du cout selon le modele
        var costPer1k = model.Contains("gpt-4") ? 0.03m :
                        model.Contains("gpt-3.5") ? 0.002m :
                        model.Contains("mistral") ? 0.001m : 0.001m;

        return new AIUsage
        {
            TenantId = tenantId,
            PromptTokens = promptTokens,
            CompletionTokens = completionTokens,
            TotalTokens = totalTokens,
            Model = model,
            Provider = provider,
            EstimatedCostUsd = (totalTokens / 1000m) * costPer1k,
            ConversationId = conversationId
        };
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Domain\Entities\AIConversation.cs" -Encoding UTF8 -Value @'
using OneLine.AI.Domain.Enums;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.AI.Domain.Entities;

/// <summary>
/// Conversation IA associee a un tenant.
/// Stocke l historique des messages pour le contexte.
///
/// Multi-tenancy :
///   Chaque tenant a ses propres conversations.
///   Un tenant ne peut pas voir les conversations d un autre.
/// </summary>
public sealed class AIConversation : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid? UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int TotalTokensUsed { get; private set; }

    private readonly List<AIMessage> _messages = [];
    public IReadOnlyList<AIMessage> Messages => _messages;

    private AIConversation() { }

    public static AIConversation Create(
        Guid tenantId,
        string title,
        Guid? userId = null)
    {
        return new AIConversation
        {
            TenantId = tenantId,
            UserId = userId,
            Title = title,
            IsActive = true,
            TotalTokensUsed = 0
        };
    }

    public AIMessage AddMessage(MessageRole role, string content)
    {
        var message = AIMessage.Create(Id, role, content);
        _messages.Add(message);
        SetUpdatedAt();
        return message;
    }

    public void AddTokensUsed(int tokens)
    {
        TotalTokensUsed += tokens;
        SetUpdatedAt();
    }

    public void Close()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Domain\Entities\AIMessage.cs" -Encoding UTF8 -Value @'
using OneLine.AI.Domain.Enums;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.AI.Domain.Entities;

/// <summary>
/// Message dans une conversation IA.
/// Role : System, User ou Assistant.
/// </summary>
public sealed class AIMessage : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public MessageRole Role { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public int? TokenCount { get; private set; }

    private AIMessage() { }

    public static AIMessage Create(
        Guid conversationId,
        MessageRole role,
        string content,
        int? tokenCount = null)
    {
        return new AIMessage
        {
            ConversationId = conversationId,
            Role = role,
            Content = content,
            TokenCount = tokenCount
        };
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Domain\Errors\AIErrors.cs" -Encoding UTF8 -Value @'
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Domain.Errors;

public static class AIErrors
{
    public static readonly Error QuotaExceeded =
        Error.Forbidden(
            "AI.QuotaExceeded",
            "Quota de tokens IA depasse pour ce mois. Mettez a niveau votre plan.");

    public static readonly Error ProviderError =
        Error.Failure(
            "AI.ProviderError",
            "Erreur lors de la communication avec le provider IA.");

    public static readonly Error ConversationNotFound =
        Error.NotFound(
            "AI.ConversationNotFound",
            "Conversation introuvable.");

    public static readonly Error InvalidMessage =
        Error.Validation(
            "AI.InvalidMessage",
            "Le message ne peut pas etre vide.");

    public static readonly Error AINotConfigured =
        Error.Failure(
            "AI.NotConfigured",
            "Le module IA n est pas configure. Verifiez votre cle API.");
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Domain\Interfaces\IAIUsageRepository.cs" -Encoding UTF8 -Value @'
using OneLine.AI.Domain.Entities;

namespace OneLine.AI.Domain.Interfaces;

public interface IAIUsageRepository
{
    Task AddAsync(AIUsage usage, CancellationToken ct = default);
    Task<int> GetMonthlyTokensAsync(Guid tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<AIUsage>> GetByTenantIdAsync(
        Guid tenantId, int limit = 50, CancellationToken ct = default);
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Domain\Interfaces\IAIConversationRepository.cs" -Encoding UTF8 -Value @'
using OneLine.AI.Domain.Entities;

namespace OneLine.AI.Domain.Interfaces;

public interface IAIConversationRepository
{
    Task<AIConversation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AIConversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AIConversation>> GetByTenantIdAsync(
        Guid tenantId, CancellationToken ct = default);
    Task AddAsync(AIConversation conversation, CancellationToken ct = default);
    void Update(AIConversation conversation);
}
'@

# ── APPLICATION ──────────────────────────────────────────────

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\Interfaces\ILLMService.cs" -Encoding UTF8 -Value @'
namespace OneLine.AI.Application.Interfaces;

/// <summary>
/// Abstraction du service LLM.
/// Pattern : Strategy — permet de changer de provider sans toucher Application.
///
/// Providers supportes :
///   - OpenAI (GPT-4o, GPT-3.5)
///   - Mistral (Mistral-7B, Mistral-Large)
///   - Groq (Llama, Mixtral — inference ultra-rapide)
///   - Ollama (modeles locaux — 100% prive)
/// </summary>
public interface ILLMService
{
    /// <summary>
    /// Envoie une liste de messages et retourne la reponse du LLM.
    /// </summary>
    Task<LLMResponse> ChatAsync(
        IReadOnlyList<LLMMessage> messages,
        string? model = null,
        CancellationToken ct = default);

    /// <summary>Nom du provider actif</summary>
    string ProviderName { get; }

    /// <summary>Modele par defaut</summary>
    string DefaultModel { get; }
}

public sealed record LLMMessage(string Role, string Content);

public sealed record LLMResponse(
    string Content,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    string Model,
    string Provider
);
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\Interfaces\IUnitOfWork.cs" -Encoding UTF8 -Value @'
namespace OneLine.AI.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\DTOs\AIDto.cs" -Encoding UTF8 -Value @'
namespace OneLine.AI.Application.DTOs;

public sealed record ChatResponseDto(
    Guid ConversationId,
    string Content,
    int TokensUsed,
    int MonthlyTokensUsed,
    int MonthlyQuota,
    string Model,
    string Provider
);

public sealed record ConversationDto(
    Guid Id,
    Guid TenantId,
    string Title,
    bool IsActive,
    int TotalTokensUsed,
    int MessageCount,
    DateTime CreatedAt
);

public sealed record AIUsageDto(
    int MonthlyTokensUsed,
    int MonthlyQuota,
    int RemainingTokens,
    decimal EstimatedCostUsd,
    bool IsQuotaExceeded
);
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\UseCases\Chat\ChatCommand.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.AI.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Application.UseCases.Chat;

/// <summary>
/// Commande pour envoyer un message a l IA.
///
/// Flow :
///   1. Verifier le quota mensuel du tenant
///   2. Recuperer ou creer la conversation
///   3. Ajouter le message utilisateur
///   4. Envoyer au LLM avec l historique
///   5. Sauvegarder la reponse + tracker les tokens
///   6. Retourner la reponse au client
/// </summary>
public sealed record ChatCommand(
    Guid TenantId,
    string Message,
    Guid? ConversationId = null,
    string? SystemPrompt = null,
    Guid? UserId = null
) : IRequest<Result<ChatResponseDto>>;
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\UseCases\Chat\ChatCommandValidator.cs" -Encoding UTF8 -Value @'
using FluentValidation;

namespace OneLine.AI.Application.UseCases.Chat;

public sealed class ChatCommandValidator : AbstractValidator<ChatCommand>
{
    public ChatCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId est obligatoire.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Le message ne peut pas etre vide.")
            .MaximumLength(10000).WithMessage("Message trop long (max 10000 chars).");
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\UseCases\Chat\ChatCommandHandler.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.AI.Application.DTOs;
using OneLine.AI.Application.Interfaces;
using OneLine.AI.Domain.Entities;
using OneLine.AI.Domain.Enums;
using OneLine.AI.Domain.Errors;
using OneLine.AI.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Application.UseCases.Chat;

public sealed class ChatCommandHandler
    : IRequestHandler<ChatCommand, Result<ChatResponseDto>>
{
    private readonly ILLMService _llmService;
    private readonly IAIConversationRepository _conversationRepo;
    private readonly IAIUsageRepository _usageRepo;
    private readonly IUnitOfWork _unitOfWork;

    // Quota par defaut : 50k tokens/mois
    private const int DefaultMonthlyQuota = 50_000;

    public ChatCommandHandler(
        ILLMService llmService,
        IAIConversationRepository conversationRepo,
        IAIUsageRepository usageRepo,
        IUnitOfWork unitOfWork)
    {
        _llmService = llmService;
        _conversationRepo = conversationRepo;
        _usageRepo = usageRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChatResponseDto>> Handle(
        ChatCommand command, CancellationToken ct)
    {
        // 1. Verifier le quota mensuel
        var monthlyTokens = await _usageRepo
            .GetMonthlyTokensAsync(command.TenantId, ct);

        if (monthlyTokens >= DefaultMonthlyQuota)
            return AIErrors.QuotaExceeded;

        // 2. Recuperer ou creer la conversation
        AIConversation conversation;

        if (command.ConversationId.HasValue)
        {
            var existing = await _conversationRepo
                .GetByIdWithMessagesAsync(command.ConversationId.Value, ct);

            if (existing is null)
                return AIErrors.ConversationNotFound;

            conversation = existing;
        }
        else
        {
            // Nouvelle conversation — titre = premiers 50 chars du message
            var title = command.Message.Length > 50
                ? command.Message[..50] + "..."
                : command.Message;

            conversation = AIConversation.Create(
                command.TenantId, title, command.UserId);

            await _conversationRepo.AddAsync(conversation, ct);
        }

        // 3. Construire les messages pour le LLM
        var messages = new List<LLMMessage>();

        // System prompt optionnel
        if (!string.IsNullOrEmpty(command.SystemPrompt))
            messages.Add(new LLMMessage("system", command.SystemPrompt));
        else
            messages.Add(new LLMMessage("system",
                "Tu es un assistant IA utile et concis. Reponds en francais."));

        // Historique de la conversation (max 10 derniers messages)
        foreach (var msg in conversation.Messages.TakeLast(10))
        {
            var role = msg.Role switch
            {
                MessageRole.User      => "user",
                MessageRole.Assistant => "assistant",
                _                     => "system"
            };
            messages.Add(new LLMMessage(role, msg.Content));
        }

        // Message actuel
        messages.Add(new LLMMessage("user", command.Message));

        // 4. Appel au LLM
        LLMResponse llmResponse;
        try
        {
            llmResponse = await _llmService.ChatAsync(messages, ct: ct);
        }
        catch (Exception)
        {
            return AIErrors.ProviderError;
        }

        // 5. Sauvegarder les messages
        conversation.AddMessage(MessageRole.User, command.Message);
        conversation.AddMessage(MessageRole.Assistant, llmResponse.Content);
        conversation.AddTokensUsed(llmResponse.TotalTokens);

        // 6. Tracker l usage
        var usage = AIUsage.Create(
            command.TenantId,
            llmResponse.PromptTokens,
            llmResponse.CompletionTokens,
            llmResponse.Model,
            llmResponse.Provider,
            conversation.Id.ToString());

        await _usageRepo.AddAsync(usage, ct);
        _conversationRepo.Update(conversation);
        await _unitOfWork.SaveChangesAsync(ct);

        return new ChatResponseDto(
            ConversationId: conversation.Id,
            Content: llmResponse.Content,
            TokensUsed: llmResponse.TotalTokens,
            MonthlyTokensUsed: monthlyTokens + llmResponse.TotalTokens,
            MonthlyQuota: DefaultMonthlyQuota,
            Model: llmResponse.Model,
            Provider: llmResponse.Provider);
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\UseCases\GetUsage\GetAIUsageQuery.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.AI.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Application.UseCases.GetUsage;

public sealed record GetAIUsageQuery(Guid TenantId)
    : IRequest<Result<AIUsageDto>>;
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\UseCases\GetUsage\GetAIUsageQueryHandler.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.AI.Application.DTOs;
using OneLine.AI.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Application.UseCases.GetUsage;

public sealed class GetAIUsageQueryHandler
    : IRequestHandler<GetAIUsageQuery, Result<AIUsageDto>>
{
    private readonly IAIUsageRepository _usageRepo;
    private const int DefaultMonthlyQuota = 50_000;

    public GetAIUsageQueryHandler(IAIUsageRepository usageRepo)
    {
        _usageRepo = usageRepo;
    }

    public async Task<Result<AIUsageDto>> Handle(
        GetAIUsageQuery query, CancellationToken ct)
    {
        var monthlyTokens = await _usageRepo
            .GetMonthlyTokensAsync(query.TenantId, ct);

        var usages = await _usageRepo
            .GetByTenantIdAsync(query.TenantId, 50, ct);

        var totalCost = usages.Sum(u => u.EstimatedCostUsd);

        return new AIUsageDto(
            MonthlyTokensUsed: monthlyTokens,
            MonthlyQuota: DefaultMonthlyQuota,
            RemainingTokens: Math.Max(0, DefaultMonthlyQuota - monthlyTokens),
            EstimatedCostUsd: totalCost,
            IsQuotaExceeded: monthlyTokens >= DefaultMonthlyQuota);
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Application\AIApplicationExtensions.cs" -Encoding UTF8 -Value @'
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OneLine.AI.Application;

public static class AIApplicationExtensions
{
    public static IServiceCollection AddAIApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(AIApplicationExtensions).Assembly;
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
'@

# ── INFRASTRUCTURE ───────────────────────────────────────────

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Options\AIOptions.cs" -Encoding UTF8 -Value @'
using OneLine.AI.Domain.Enums;

namespace OneLine.AI.Infrastructure.Options;

public sealed class AIOptions
{
    public const string SectionName = "AI";
    public AIProvider Provider { get; set; } = AIProvider.OpenAI;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
    public string BaseUrl { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 2000;
    public float Temperature { get; set; } = 0.7f;
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Persistence\AIDbContext.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using OneLine.AI.Domain.Entities;

namespace OneLine.AI.Infrastructure.Persistence;

public sealed class AIDbContext : DbContext
{
    public AIDbContext(DbContextOptions<AIDbContext> options)
        : base(options) { }

    public DbSet<AIConversation> Conversations => Set<AIConversation>();
    public DbSet<AIMessage> Messages => Set<AIMessage>();
    public DbSet<AIUsage> Usages => Set<AIUsage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("ai");

        builder.Entity<AIConversation>(e =>
        {
            e.ToTable("conversations");
            e.HasKey(c => c.Id);
            e.Property(c => c.Title).HasMaxLength(200).IsRequired();
            e.HasIndex(c => c.TenantId).HasDatabaseName("ix_conversations_tenant_id");
            e.HasMany(c => c.Messages)
             .WithOne()
             .HasForeignKey(m => m.ConversationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AIMessage>(e =>
        {
            e.ToTable("messages");
            e.HasKey(m => m.Id);
            e.Property(m => m.Content).IsRequired();
            e.Property(m => m.Role).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(m => m.ConversationId)
             .HasDatabaseName("ix_messages_conversation_id");
        });

        builder.Entity<AIUsage>(e =>
        {
            e.ToTable("usages");
            e.HasKey(u => u.Id);
            e.Property(u => u.Model).HasMaxLength(50).IsRequired();
            e.Property(u => u.Provider).HasMaxLength(20).IsRequired();
            e.Property(u => u.EstimatedCostUsd).HasPrecision(10, 6);
            e.Property(u => u.ConversationId).HasMaxLength(100);
            e.HasIndex(u => u.TenantId).HasDatabaseName("ix_usages_tenant_id");
            e.HasIndex(u => new { u.TenantId, u.CreatedAt })
             .HasDatabaseName("ix_usages_tenant_date");
        });
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Persistence\AIDbContextFactory.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OneLine.AI.Infrastructure.Persistence;

public sealed class AIDbContextFactory
    : IDesignTimeDbContextFactory<AIDbContext>
{
    public AIDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AIDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=oneline_saaskit;Username=postgres;Password=postgres");
        return new AIDbContext(optionsBuilder.Options);
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Persistence\AIUnitOfWork.cs" -Encoding UTF8 -Value @'
using OneLine.AI.Application.Interfaces;

namespace OneLine.AI.Infrastructure.Persistence;

public sealed class AIUnitOfWork : IUnitOfWork
{
    private readonly AIDbContext _context;
    public AIUnitOfWork(AIDbContext context) => _context = context;
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Persistence\Repositories\AIUsageRepository.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using OneLine.AI.Domain.Entities;
using OneLine.AI.Domain.Interfaces;

namespace OneLine.AI.Infrastructure.Persistence.Repositories;

public sealed class AIUsageRepository : IAIUsageRepository
{
    private readonly AIDbContext _context;
    public AIUsageRepository(AIDbContext context) => _context = context;

    public async Task AddAsync(AIUsage usage, CancellationToken ct = default)
        => await _context.Usages.AddAsync(usage, ct);

    public async Task<int> GetMonthlyTokensAsync(
        Guid tenantId, CancellationToken ct = default)
    {
        var firstDayOfMonth = new DateTime(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Month, 1,
            0, 0, 0, DateTimeKind.Utc);

        return await _context.Usages
            .Where(u => u.TenantId == tenantId
                     && u.CreatedAt >= firstDayOfMonth)
            .SumAsync(u => u.TotalTokens, ct);
    }

    public async Task<IReadOnlyList<AIUsage>> GetByTenantIdAsync(
        Guid tenantId, int limit = 50, CancellationToken ct = default)
        => await _context.Usages
            .Where(u => u.TenantId == tenantId)
            .OrderByDescending(u => u.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Persistence\Repositories\AIConversationRepository.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using OneLine.AI.Domain.Entities;
using OneLine.AI.Domain.Interfaces;

namespace OneLine.AI.Infrastructure.Persistence.Repositories;

public sealed class AIConversationRepository : IAIConversationRepository
{
    private readonly AIDbContext _context;
    public AIConversationRepository(AIDbContext context) => _context = context;

    public async Task<AIConversation?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
        => await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<AIConversation?> GetByIdWithMessagesAsync(
        Guid id, CancellationToken ct = default)
        => await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<AIConversation>> GetByTenantIdAsync(
        Guid tenantId, CancellationToken ct = default)
        => await _context.Conversations
            .Where(c => c.TenantId == tenantId && c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(
        AIConversation conversation, CancellationToken ct = default)
        => await _context.Conversations.AddAsync(conversation, ct);

    public void Update(AIConversation conversation)
        => _context.Conversations.Update(conversation);
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Services\OpenAILLMService.cs" -Encoding UTF8 -Value @'
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OneLine.AI.Application.Interfaces;
using OneLine.AI.Infrastructure.Options;
using OpenAI.Chat;

namespace OneLine.AI.Infrastructure.Services;

/// <summary>
/// Implementation OpenAI du service LLM.
/// Utilise le SDK Azure.AI.OpenAI qui supporte aussi OpenAI standard.
///
/// Modeles supportes :
///   gpt-4o       -> le plus capable
///   gpt-4o-mini  -> rapide et economique (defaut)
///   gpt-3.5-turbo -> ultra-economique
///
/// Pattern : Strategy (via ILLMService)
/// </summary>
public sealed class OpenAILLMService : ILLMService
{
    private readonly AIOptions _options;
    private readonly ChatClient _chatClient;

    public string ProviderName => "OpenAI";
    public string DefaultModel => _options.Model;

    public OpenAILLMService(IOptions<AIOptions> options)
    {
        _options = options.Value;

        var client = new OpenAIClient(_options.ApiKey);
        _chatClient = client.GetChatClient(_options.Model);
    }

    public async Task<LLMResponse> ChatAsync(
        IReadOnlyList<LLMMessage> messages,
        string? model = null,
        CancellationToken ct = default)
    {
        var chatMessages = messages.Select<LLMMessage, ChatMessage>(m =>
            m.Role switch
            {
                "system"    => ChatMessage.CreateSystemMessage(m.Content),
                "assistant" => ChatMessage.CreateAssistantMessage(m.Content),
                _           => ChatMessage.CreateUserMessage(m.Content)
            }).ToList();

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = _options.MaxTokens,
            Temperature = _options.Temperature
        };

        var completion = await _chatClient.CompleteChatAsync(chatMessages, options, ct);

        var content = completion.Value.Content[0].Text;
        var usage = completion.Value.Usage;

        return new LLMResponse(
            Content: content,
            PromptTokens: usage.InputTokenCount,
            CompletionTokens: usage.OutputTokenCount,
            TotalTokens: usage.TotalTokenCount,
            Model: model ?? _options.Model,
            Provider: ProviderName);
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Services\MockLLMService.cs" -Encoding UTF8 -Value @'
using OneLine.AI.Application.Interfaces;

namespace OneLine.AI.Infrastructure.Services;

/// <summary>
/// Service LLM mock pour le developpement et les tests.
/// Retourne des reponses simulees sans appel API reel.
///
/// Utilisation :
///   - Quand AI:ApiKey est vide dans appsettings.json
///   - En environnement de test
///   - Pour tester le flow sans consommer des tokens
/// </summary>
public sealed class MockLLMService : ILLMService
{
    public string ProviderName => "Mock";
    public string DefaultModel => "mock-model";

    private static readonly string[] MockResponses =
    [
        "Bonjour ! Je suis l assistant IA OneLine. Comment puis-je vous aider ?",
        "C est une excellente question. Voici ma reponse simulee pour les tests.",
        "En mode developpement, je simule les reponses pour eviter les couts API.",
        "Pour utiliser un vrai LLM, configurez AI:ApiKey dans appsettings.json.",
        "Je peux vous aider avec de nombreuses taches : analyse, redaction, code..."
    ];

    private static int _responseIndex = 0;

    public Task<LLMResponse> ChatAsync(
        IReadOnlyList<LLMMessage> messages,
        string? model = null,
        CancellationToken ct = default)
    {
        var response = MockResponses[_responseIndex % MockResponses.Length];
        _responseIndex++;

        // Simuler les tokens (approximation : 1 token ~ 4 chars)
        var promptTokens = messages.Sum(m => m.Content.Length / 4);
        var completionTokens = response.Length / 4;

        return Task.FromResult(new LLMResponse(
            Content: response,
            PromptTokens: promptTokens,
            CompletionTokens: completionTokens,
            TotalTokens: promptTokens + completionTokens,
            Model: model ?? DefaultModel,
            Provider: ProviderName));
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\Middleware\AIQuotaMiddleware.cs" -Encoding UTF8 -Value @'
using Microsoft.AspNetCore.Http;
using OneLine.AI.Domain.Interfaces;
using OneLine.Shared.Domain.Interfaces;

namespace OneLine.AI.Infrastructure.Middleware;

/// <summary>
/// Middleware qui verifie le quota de tokens IA avant chaque requete AI.
/// Retourne HTTP 429 si le quota mensuel est depasse.
///
/// Ne s applique qu aux routes /api/ai/*
/// Les autres routes ne sont pas affectees.
/// </summary>
public sealed class AIQuotaMiddleware
{
    private readonly RequestDelegate _next;
    private const int DefaultMonthlyQuota = 50_000;

    public AIQuotaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenant currentTenant,
        IAIUsageRepository usageRepo)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Appliquer uniquement aux routes AI
        if (!path.StartsWith("/api/ai", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!currentTenant.IsResolved)
        {
            await _next(context);
            return;
        }

        var monthlyTokens = await usageRepo
            .GetMonthlyTokensAsync(currentTenant.TenantId);

        if (monthlyTokens >= DefaultMonthlyQuota)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                $"{{\"code\":\"AI.QuotaExceeded\"," +
                $"\"message\":\"Quota mensuel de {DefaultMonthlyQuota} tokens depasse.\"," +
                $"\"used\":{monthlyTokens},\"quota\":{DefaultMonthlyQuota}}}");
            return;
        }

        await _next(context);
    }
}
'@

Set-Content -Path "src\Modules\AI\OneLine.AI.Infrastructure\AIInfrastructureExtensions.cs" -Encoding UTF8 -Value @'
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

        // LLM Service — Mock si pas de cle API, OpenAI sinon
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
'@

# ── API Controller ───────────────────────────────────────────
Set-Content -Path "src\OneLine.API\Controllers\AIController.cs" -Encoding UTF8 -Value @'
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneLine.AI.Application.DTOs;
using OneLine.AI.Application.UseCases.Chat;
using OneLine.AI.Application.UseCases.GetUsage;
using OneLine.Shared.Domain.Result;

namespace OneLine.API.Controllers;

[ApiController]
[Route("api/ai")]
[Produces("application/json")]
public sealed class AIController : ControllerBase
{
    private readonly ISender _sender;

    public AIController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Envoyer un message a l IA et recevoir une reponse.
    /// Supporte les conversations multi-tours via ConversationId.
    /// </summary>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(ChatResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Chat(
        [FromBody] ChatCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
    }

    /// <summary>
    /// Obtenir les statistiques d utilisation IA du tenant.
    /// Tokens utilises, quota mensuel, cout estime.
    /// </summary>
    [HttpGet("usage/{tenantId:guid}")]
    [ProducesResponseType(typeof(AIUsageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsage(
        Guid tenantId, CancellationToken ct)
    {
        var result = await _sender.Send(
            new GetAIUsageQuery(tenantId), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
    }

    private IActionResult HandleError(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound   => NotFound(new { error.Code, error.Message }),
            ErrorType.Forbidden  => StatusCode(429, new { error.Code, error.Message }),
            ErrorType.Validation => BadRequest(new { error.Code, error.Message }),
            _                    => StatusCode(500, new { error.Code, error.Message })
        };
}
'@

# ── appsettings.json ─────────────────────────────────────────
Set-Content -Path "src\OneLine.API\appsettings.json" -Encoding UTF8 -Value @'
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=oneline_saaskit;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "SecretKey": "OneLine-SuperSecret-Key-2025-MinimumLength32Chars!",
    "Issuer": "OneLine.API",
    "Audience": "OneLine.Client",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "Stripe": {
    "SecretKey": "sk_test_YOUR_KEY_HERE",
    "PublishableKey": "pk_test_YOUR_KEY_HERE",
    "WebhookSecret": "whsec_YOUR_SECRET_HERE"
  },
  "Security": {
    "MaxRequestsPerMinutePerIp": 60,
    "MaxRequestsPerMinutePerUser": 100,
    "MaxFailedLoginAttempts": 5,
    "LockoutDurationMinutes": 15
  },
  "AI": {
    "Provider": "OpenAI",
    "ApiKey": "sk-YOUR_KEY_HERE",
    "Model": "gpt-4o-mini",
    "MaxTokens": 2000,
    "Temperature": "0.7"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
'@

# ── Program.cs ───────────────────────────────────────────────
Set-Content -Path "src\OneLine.API\Program.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.AI.Application;
using OneLine.AI.Application.UseCases.Chat;
using OneLine.AI.Infrastructure;
using OneLine.AI.Infrastructure.Middleware;
using OneLine.Auth.Application;
using OneLine.Auth.Infrastructure;
using OneLine.Billing.Application;
using OneLine.Billing.Application.UseCases.CreateSubscription;
using OneLine.Billing.Infrastructure;
using OneLine.Billing.Infrastructure.Middleware;
using OneLine.Observability.Infrastructure;
using OneLine.Observability.Infrastructure.Middleware;
using OneLine.Security.Infrastructure;
using OneLine.Security.Infrastructure.Middleware;
using OneLine.Security.Infrastructure.RateLimiting;
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
builder.Services.AddSecurityInfrastructure(builder.Configuration);
builder.Services.AddObservabilityInfrastructure();
builder.Services.AddAIApplication();
builder.Services.AddAIInfrastructure(builder.Configuration);

// MediatR explicite pour tous les modules
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateSubscriptionCommand).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(ChatCommand).Assembly);
});

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

// ── Middleware Pipeline ───────────────────────────────────────
app.UseObservability();
app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SubscriptionMiddleware>();
app.UseMiddleware<AIQuotaMiddleware>();
app.MapControllers();

app.Run();
'@

Write-Host "Fichiers crees" -ForegroundColor Green

# ── ETAPE 5 : Build ──────────────────────────────────────────
Write-Host "`n[5/6] Build initial..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n=== BUILD ECHOUE - voir erreurs ci-dessus ===" -ForegroundColor Red
    exit 1
}

Write-Host "`n=== BUILD REUSSI ===" -ForegroundColor Green

# ── ETAPE 6 : Migrations ─────────────────────────────────────
Write-Host "`n[6/6] Migration AI..." -ForegroundColor Yellow

dotnet ef migrations add InitialAI `
  --project src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  --startup-project tools\OneLine.Migrations\OneLine.Migrations.csproj `
  --context AIDbContext `
  --output-dir Persistence\Migrations

dotnet ef database update `
  --project src\Modules\AI\OneLine.AI.Infrastructure\OneLine.AI.Infrastructure.csproj `
  --startup-project tools\OneLine.Migrations\OneLine.Migrations.csproj `
  --context AIDbContext

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n=== MODULE AI COMPLET ===" -ForegroundColor Green
    Write-Host "`nLancer l API :" -ForegroundColor Cyan
    Write-Host "dotnet run --project src\OneLine.API\OneLine.API.csproj" -ForegroundColor Gray
    Write-Host "`nTest chat (mode Mock sans cle API) :" -ForegroundColor Cyan
    Write-Host "POST /api/ai/chat avec { tenantId, message }" -ForegroundColor Gray
} else {
    Write-Host "`n=== MIGRATION ECHOUEE ===" -ForegroundColor Red
}
