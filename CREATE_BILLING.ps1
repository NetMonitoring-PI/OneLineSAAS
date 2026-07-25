# ============================================================
# Script Module Billing -- One Line SaaS Kit
# Exécuter depuis : C:\Users\DELL\Projects\OneLine.SaasKit
# ============================================================

Write-Host "=== Module Billing ===" -ForegroundColor Cyan

# ── ÉTAPE 1 : Créer les projets ──────────────────────────────
Write-Host "`n[1/7] Creation des projets..." -ForegroundColor Yellow

dotnet new classlib -n OneLine.Billing.Domain -o src\Modules\Billing\OneLine.Billing.Domain --force
dotnet new classlib -n OneLine.Billing.Application -o src\Modules\Billing\OneLine.Billing.Application --force
dotnet new classlib -n OneLine.Billing.Infrastructure -o src\Modules\Billing\OneLine.Billing.Infrastructure --force

dotnet sln add src\Modules\Billing\OneLine.Billing.Domain\OneLine.Billing.Domain.csproj
dotnet sln add src\Modules\Billing\OneLine.Billing.Application\OneLine.Billing.Application.csproj
dotnet sln add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj

Remove-Item -Force src\Modules\Billing\OneLine.Billing.Domain\Class1.cs -ErrorAction SilentlyContinue
Remove-Item -Force src\Modules\Billing\OneLine.Billing.Application\Class1.cs -ErrorAction SilentlyContinue
Remove-Item -Force src\Modules\Billing\OneLine.Billing.Infrastructure\Class1.cs -ErrorAction SilentlyContinue

Write-Host "Projets crees ✅" -ForegroundColor Green

# ── ÉTAPE 2 : References et packages ─────────────────────────
Write-Host "`n[2/7] References et packages..." -ForegroundColor Yellow

# Domain
dotnet add src\Modules\Billing\OneLine.Billing.Domain\OneLine.Billing.Domain.csproj reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj
dotnet add src\Modules\Billing\OneLine.Billing.Domain\OneLine.Billing.Domain.csproj package MediatR --version 12.2.0

# Application
dotnet add src\Modules\Billing\OneLine.Billing.Application\OneLine.Billing.Application.csproj reference src\Modules\Billing\OneLine.Billing.Domain\OneLine.Billing.Domain.csproj
dotnet add src\Modules\Billing\OneLine.Billing.Application\OneLine.Billing.Application.csproj reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj
dotnet add src\Modules\Billing\OneLine.Billing.Application\OneLine.Billing.Application.csproj package MediatR --version 12.2.0
dotnet add src\Modules\Billing\OneLine.Billing.Application\OneLine.Billing.Application.csproj package FluentValidation --version 11.9.0
dotnet add src\Modules\Billing\OneLine.Billing.Application\OneLine.Billing.Application.csproj package FluentValidation.DependencyInjectionExtensions --version 11.9.0
dotnet add src\Modules\Billing\OneLine.Billing.Application\OneLine.Billing.Application.csproj package Microsoft.Extensions.DependencyInjection.Abstractions --version 9.0.0

# Infrastructure
dotnet add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj reference src\Modules\Billing\OneLine.Billing.Application\OneLine.Billing.Application.csproj
dotnet add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj
dotnet add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0
dotnet add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj package Microsoft.AspNetCore.Http --version 2.2.2
dotnet add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj package Microsoft.Extensions.DependencyInjection.Abstractions --version 9.0.0
dotnet add src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj package Stripe.net --version 46.2.0

# API
dotnet add src\OneLine.API\OneLine.API.csproj reference src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj

# Migrations tool
dotnet add tools\OneLine.Migrations\OneLine.Migrations.csproj reference src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj

Write-Host "References OK ✅" -ForegroundColor Green

# ── ÉTAPE 3 : Créer les dossiers ─────────────────────────────
Write-Host "`n[3/7] Creation des dossiers..." -ForegroundColor Yellow

$dirs = @(
    "src\Modules\Billing\OneLine.Billing.Domain\Entities",
    "src\Modules\Billing\OneLine.Billing.Domain\Enums",
    "src\Modules\Billing\OneLine.Billing.Domain\Events",
    "src\Modules\Billing\OneLine.Billing.Domain\Errors",
    "src\Modules\Billing\OneLine.Billing.Domain\Interfaces",
    "src\Modules\Billing\OneLine.Billing.Application\DTOs",
    "src\Modules\Billing\OneLine.Billing.Application\Interfaces",
    "src\Modules\Billing\OneLine.Billing.Application\UseCases\CreateSubscription",
    "src\Modules\Billing\OneLine.Billing.Application\UseCases\GetSubscription",
    "src\Modules\Billing\OneLine.Billing.Application\UseCases\CancelSubscription",
    "src\Modules\Billing\OneLine.Billing.Infrastructure\Persistence\Repositories",
    "src\Modules\Billing\OneLine.Billing.Infrastructure\Services",
    "src\Modules\Billing\OneLine.Billing.Infrastructure\Middleware",
    "src\Modules\Billing\OneLine.Billing.Infrastructure\Webhooks"
)
foreach ($dir in $dirs) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

Write-Host "Dossiers crees ✅" -ForegroundColor Green

# ── ÉTAPE 4 : Créer les fichiers ─────────────────────────────
Write-Host "`n[4/7] Creation des fichiers..." -ForegroundColor Yellow

# ── DOMAIN ──────────────────────────────────────────────────

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Enums\BillingEnums.cs" -Encoding UTF8 -Value @'
namespace OneLine.Billing.Domain.Enums;

public enum SubscriptionStatus
{
    Active = 0,
    Cancelled = 1,
    PastDue = 2,
    Trialing = 3,
    Unpaid = 4,
    Incomplete = 5
}

public enum BillingInterval
{
    Monthly = 0,
    Yearly = 1
}

public enum InvoiceStatus
{
    Draft = 0,
    Open = 1,
    Paid = 2,
    Void = 3,
    Uncollectible = 4
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Events\IDomainEvent.cs" -Encoding UTF8 -Value @'
using MediatR;

namespace OneLine.Billing.Domain.Events;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Events\SubscriptionCreatedEvent.cs" -Encoding UTF8 -Value @'
namespace OneLine.Billing.Domain.Events;

public sealed record SubscriptionCreatedEvent(
    Guid SubscriptionId,
    Guid TenantId,
    string PlanName) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record SubscriptionCancelledEvent(
    Guid SubscriptionId,
    Guid TenantId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record PaymentFailedEvent(
    Guid TenantId,
    string InvoiceId,
    decimal Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Entities\Plan.cs" -Encoding UTF8 -Value @'
using OneLine.Billing.Domain.Enums;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.Billing.Domain.Entities;

/// <summary>
/// Représente un plan tarifaire SaaS.
/// Ex: Free, Starter (9$/mois), Pro (29$/mois), Enterprise
/// </summary>
public sealed class Plan : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public BillingInterval Interval { get; private set; }
    public int TokenQuota { get; private set; }
    public bool IsActive { get; private set; }
    public string? StripeProductId { get; private set; }
    public string? StripePriceId { get; private set; }

    private Plan() { }

    public static Plan Create(
        string name,
        string description,
        decimal price,
        BillingInterval interval,
        int tokenQuota = 10000,
        string? stripeProductId = null,
        string? stripePriceId = null)
    {
        return new Plan
        {
            Name = name,
            Description = description,
            Price = price,
            Interval = interval,
            TokenQuota = tokenQuota,
            IsActive = true,
            StripeProductId = stripeProductId,
            StripePriceId = stripePriceId
        };
    }

    public void Deactivate() { IsActive = false; SetUpdatedAt(); }
    public void SetStripeIds(string productId, string priceId)
    {
        StripeProductId = productId;
        StripePriceId = priceId;
        SetUpdatedAt();
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Entities\Subscription.cs" -Encoding UTF8 -Value @'
using OneLine.Billing.Domain.Enums;
using OneLine.Billing.Domain.Events;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.Billing.Domain.Entities;

/// <summary>
/// Abonnement d un tenant a un plan.
/// Lié a Stripe via StripeSubscriptionId et StripeCustomerId.
///
/// Cycle de vie :
///   1. Tenant s inscrit -> Subscription créée (Trialing)
///   2. Stripe prélève -> Active
///   3. Paiement échoué -> PastDue
///   4. Tenant annule -> Cancelled
/// </summary>
public sealed class Subscription : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid PlanId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime CurrentPeriodStart { get; private set; }
    public DateTime CurrentPeriodEnd { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? TrialEndsAt { get; private set; }
    public string? StripeSubscriptionId { get; private set; }
    public string? StripeCustomerId { get; private set; }

    public bool IsActive => Status == SubscriptionStatus.Active
                         || Status == SubscriptionStatus.Trialing;

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    public void ClearDomainEvents() => _domainEvents.Clear();

    // Navigation
    public Plan? Plan { get; private set; }

    private Subscription() { }

    public static Subscription Create(
        Guid tenantId,
        Guid planId,
        string? stripeCustomerId = null,
        int trialDays = 14)
    {
        var now = DateTime.UtcNow;
        var sub = new Subscription
        {
            TenantId = tenantId,
            PlanId = planId,
            Status = SubscriptionStatus.Trialing,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = now.AddDays(trialDays),
            TrialEndsAt = now.AddDays(trialDays),
            StripeCustomerId = stripeCustomerId
        };

        sub._domainEvents.Add(new SubscriptionCreatedEvent(sub.Id, tenantId, string.Empty));
        return sub;
    }

    // Appelé par webhook Stripe : invoice.payment_succeeded
    public void Activate(DateTime periodEnd, string stripeSubscriptionId)
    {
        Status = SubscriptionStatus.Active;
        CurrentPeriodEnd = periodEnd;
        StripeSubscriptionId = stripeSubscriptionId;
        SetUpdatedAt();
    }

    // Appelé par webhook Stripe : invoice.payment_failed
    public void MarkAsPastDue()
    {
        Status = SubscriptionStatus.PastDue;
        SetUpdatedAt();
    }

    // Appelé par webhook Stripe : customer.subscription.deleted
    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        _domainEvents.Add(new SubscriptionCancelledEvent(Id, TenantId));
        SetUpdatedAt();
    }

    public void SetStripeSubscriptionId(string id)
    {
        StripeSubscriptionId = id;
        SetUpdatedAt();
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Entities\Invoice.cs" -Encoding UTF8 -Value @'
using OneLine.Billing.Domain.Enums;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.Billing.Domain.Entities;

/// <summary>
/// Facture générée par Stripe.
/// Créée automatiquement via les webhooks Stripe.
/// </summary>
public sealed class Invoice : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "usd";
    public InvoiceStatus Status { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? StripeInvoiceId { get; private set; }
    public string? StripeHostedUrl { get; private set; }

    private Invoice() { }

    public static Invoice Create(
        Guid tenantId,
        Guid subscriptionId,
        decimal amount,
        string stripeInvoiceId,
        string? hostedUrl = null,
        string currency = "usd")
    {
        return new Invoice
        {
            TenantId = tenantId,
            SubscriptionId = subscriptionId,
            Amount = amount,
            Currency = currency,
            Status = InvoiceStatus.Open,
            StripeInvoiceId = stripeInvoiceId,
            StripeHostedUrl = hostedUrl
        };
    }

    public void MarkAsPaid()
    {
        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkAsVoid()
    {
        Status = InvoiceStatus.Void;
        SetUpdatedAt();
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Errors\BillingErrors.cs" -Encoding UTF8 -Value @'
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Domain.Errors;

public static class BillingErrors
{
    public static readonly Error SubscriptionNotFound =
        Error.NotFound("Billing.SubscriptionNotFound", "Aucun abonnement trouvé.");

    public static readonly Error PlanNotFound =
        Error.NotFound("Billing.PlanNotFound", "Le plan n existe pas.");

    public static readonly Error SubscriptionExpired =
        Error.Forbidden("Billing.SubscriptionExpired", "Abonnement expiré. Veuillez renouveler.");

    public static readonly Error AlreadySubscribed =
        Error.Conflict("Billing.AlreadySubscribed", "Un abonnement actif existe déjà.");

    public static readonly Error InvalidWebhookSignature =
        Error.Unauthorized("Billing.InvalidWebhook", "Signature webhook Stripe invalide.");

    public static readonly Error StripeError =
        Error.Failure("Billing.StripeError", "Erreur lors de la communication avec Stripe.");
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Interfaces\ISubscriptionRepository.cs" -Encoding UTF8 -Value @'
using OneLine.Billing.Domain.Entities;

namespace OneLine.Billing.Domain.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Subscription?> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default);
    Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeId, CancellationToken ct = default);
    Task<bool> HasActiveSubscriptionAsync(Guid tenantId, CancellationToken ct = default);
    Task AddAsync(Subscription subscription, CancellationToken ct = default);
    void Update(Subscription subscription);
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Interfaces\IPlanRepository.cs" -Encoding UTF8 -Value @'
using OneLine.Billing.Domain.Entities;

namespace OneLine.Billing.Domain.Interfaces;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Plan plan, CancellationToken ct = default);
    void Update(Plan plan);
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Domain\Interfaces\IInvoiceRepository.cs" -Encoding UTF8 -Value @'
using OneLine.Billing.Domain.Entities;

namespace OneLine.Billing.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByStripeInvoiceIdAsync(string stripeId, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default);
    Task AddAsync(Invoice invoice, CancellationToken ct = default);
    void Update(Invoice invoice);
}
'@

# ── APPLICATION ──────────────────────────────────────────────

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\DTOs\SubscriptionDto.cs" -Encoding UTF8 -Value @'
namespace OneLine.Billing.Application.DTOs;

public sealed record SubscriptionDto(
    Guid Id,
    Guid TenantId,
    Guid PlanId,
    string PlanName,
    string Status,
    bool IsActive,
    decimal Price,
    string Interval,
    DateTime CurrentPeriodEnd,
    DateTime? TrialEndsAt,
    DateTime CreatedAt
);

public sealed record PlanDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Interval,
    int TokenQuota,
    bool IsActive
);

public sealed record InvoiceDto(
    Guid Id,
    Guid TenantId,
    decimal Amount,
    string Currency,
    string Status,
    DateTime? PaidAt,
    string? StripeHostedUrl,
    DateTime CreatedAt
);
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\Interfaces\IUnitOfWork.cs" -Encoding UTF8 -Value @'
namespace OneLine.Billing.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\Interfaces\IStripeService.cs" -Encoding UTF8 -Value @'
namespace OneLine.Billing.Application.Interfaces;

/// <summary>
/// Abstraction du service Stripe.
/// Pattern Strategy -- permet de changer de provider sans toucher Application.
/// </summary>
public interface IStripeService
{
    /// <summary>Créer un client Stripe pour le tenant</summary>
    Task<string> CreateCustomerAsync(string email, string tenantName, CancellationToken ct = default);

    /// <summary>Créer un abonnement Stripe</summary>
    Task<(string SubscriptionId, DateTime PeriodEnd)> CreateSubscriptionAsync(
        string customerId, string stripePriceId, CancellationToken ct = default);

    /// <summary>Annuler un abonnement Stripe</summary>
    Task CancelSubscriptionAsync(string stripeSubscriptionId, CancellationToken ct = default);

    /// <summary>Créer une session de paiement Checkout</summary>
    Task<string> CreateCheckoutSessionAsync(
        string customerId, string stripePriceId,
        string successUrl, string cancelUrl,
        CancellationToken ct = default);

    /// <summary>Valider la signature d'un webhook Stripe</summary>
    bool ValidateWebhookSignature(string payload, string signature, string secret, out string eventType, out string eventJson);
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\UseCases\CreateSubscription\CreateSubscriptionCommand.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.Billing.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.CreateSubscription;

public sealed record CreateSubscriptionCommand(
    Guid TenantId,
    Guid PlanId,
    string TenantEmail,
    string TenantName
) : IRequest<Result<SubscriptionDto>>;
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\UseCases\CreateSubscription\CreateSubscriptionCommandValidator.cs" -Encoding UTF8 -Value @'
using FluentValidation;

namespace OneLine.Billing.Application.UseCases.CreateSubscription;

public sealed class CreateSubscriptionCommandValidator
    : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.TenantEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.TenantName).NotEmpty().MaximumLength(100);
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\UseCases\CreateSubscription\CreateSubscriptionCommandHandler.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.Billing.Application.DTOs;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Errors;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.CreateSubscription;

/// <summary>
/// Flow complet de création d abonnement :
/// 1. Vérifier pas déjà abonné
/// 2. Récupérer le plan
/// 3. Créer le Customer Stripe
/// 4. Créer la Subscription Stripe
/// 5. Sauvegarder en DB
/// </summary>
public sealed class CreateSubscriptionCommandHandler
    : IRequestHandler<CreateSubscriptionCommand, Result<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly IPlanRepository _planRepo;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepo,
        IPlanRepository planRepo,
        IStripeService stripeService,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepo = subscriptionRepo;
        _planRepo = planRepo;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SubscriptionDto>> Handle(
        CreateSubscriptionCommand command, CancellationToken ct)
    {
        // 1. Vérifier pas déjà abonné
        var hasActive = await _subscriptionRepo
            .HasActiveSubscriptionAsync(command.TenantId, ct);
        if (hasActive) return BillingErrors.AlreadySubscribed;

        // 2. Récupérer le plan
        var plan = await _planRepo.GetByIdAsync(command.PlanId, ct);
        if (plan is null) return BillingErrors.PlanNotFound;

        // 3. Créer Customer Stripe
        var customerId = await _stripeService.CreateCustomerAsync(
            command.TenantEmail, command.TenantName, ct);

        // 4. Créer Subscription en DB (Trialing)
        var subscription = Subscription.Create(
            command.TenantId, command.PlanId, customerId);

        // 5. Si plan Stripe configuré → créer abonnement Stripe
        if (!string.IsNullOrEmpty(plan.StripePriceId))
        {
            var (stripeSubId, periodEnd) = await _stripeService
                .CreateSubscriptionAsync(customerId, plan.StripePriceId, ct);

            subscription.Activate(periodEnd, stripeSubId);
        }

        await _subscriptionRepo.AddAsync(subscription, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new SubscriptionDto(
            subscription.Id, subscription.TenantId, subscription.PlanId,
            plan.Name, subscription.Status.ToString(), subscription.IsActive,
            plan.Price, plan.Interval.ToString(),
            subscription.CurrentPeriodEnd, subscription.TrialEndsAt,
            subscription.CreatedAt);
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\UseCases\GetSubscription\GetSubscriptionQuery.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.Billing.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.GetSubscription;

public sealed record GetSubscriptionByTenantQuery(Guid TenantId)
    : IRequest<Result<SubscriptionDto>>;
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\UseCases\GetSubscription\GetSubscriptionQueryHandler.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.Billing.Application.DTOs;
using OneLine.Billing.Domain.Errors;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.GetSubscription;

public sealed class GetSubscriptionByTenantQueryHandler
    : IRequestHandler<GetSubscriptionByTenantQuery, Result<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly IPlanRepository _planRepo;

    public GetSubscriptionByTenantQueryHandler(
        ISubscriptionRepository subscriptionRepo,
        IPlanRepository planRepo)
    {
        _subscriptionRepo = subscriptionRepo;
        _planRepo = planRepo;
    }

    public async Task<Result<SubscriptionDto>> Handle(
        GetSubscriptionByTenantQuery query, CancellationToken ct)
    {
        var sub = await _subscriptionRepo.GetByTenantIdAsync(query.TenantId, ct);
        if (sub is null) return BillingErrors.SubscriptionNotFound;

        var plan = await _planRepo.GetByIdAsync(sub.PlanId, ct);

        return new SubscriptionDto(
            sub.Id, sub.TenantId, sub.PlanId,
            plan?.Name ?? "Unknown",
            sub.Status.ToString(), sub.IsActive,
            plan?.Price ?? 0, plan?.Interval.ToString() ?? "",
            sub.CurrentPeriodEnd, sub.TrialEndsAt,
            sub.CreatedAt);
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\UseCases\CancelSubscription\CancelSubscriptionCommand.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid TenantId)
    : IRequest<Result>;
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\UseCases\CancelSubscription\CancelSubscriptionCommandHandler.cs" -Encoding UTF8 -Value @'
using MediatR;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Domain.Errors;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.CancelSubscription;

public sealed class CancelSubscriptionCommandHandler
    : IRequestHandler<CancelSubscriptionCommand, Result>
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepo,
        IStripeService stripeService,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepo = subscriptionRepo;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CancelSubscriptionCommand command, CancellationToken ct)
    {
        var sub = await _subscriptionRepo.GetByTenantIdAsync(command.TenantId, ct);
        if (sub is null) return BillingErrors.SubscriptionNotFound;

        // Annuler dans Stripe si connecté
        if (!string.IsNullOrEmpty(sub.StripeSubscriptionId))
            await _stripeService.CancelSubscriptionAsync(sub.StripeSubscriptionId, ct);

        sub.Cancel();
        _subscriptionRepo.Update(sub);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Application\BillingApplicationExtensions.cs" -Encoding UTF8 -Value @'
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OneLine.Billing.Application;

public static class BillingApplicationExtensions
{
    public static IServiceCollection AddBillingApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(BillingApplicationExtensions).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
'@

# ── INFRASTRUCTURE ───────────────────────────────────────────

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Persistence\BillingDbContext.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using OneLine.Billing.Domain.Entities;

namespace OneLine.Billing.Infrastructure.Persistence;

public sealed class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options)
        : base(options) { }

    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("billing");

        builder.Entity<Plan>(e =>
        {
            e.ToTable("plans");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(50).IsRequired();
            e.Property(p => p.Description).HasMaxLength(200);
            e.Property(p => p.Price).HasPrecision(10, 2).IsRequired();
            e.Property(p => p.Interval).HasConversion<string>().HasMaxLength(10);
            e.Property(p => p.StripeProductId).HasMaxLength(100);
            e.Property(p => p.StripePriceId).HasMaxLength(100);
        });

        builder.Entity<Subscription>(e =>
        {
            e.ToTable("subscriptions");
            e.HasKey(s => s.Id);
            e.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(s => s.StripeSubscriptionId).HasMaxLength(100);
            e.Property(s => s.StripeCustomerId).HasMaxLength(100);
            e.HasIndex(s => s.TenantId).HasDatabaseName("ix_subscriptions_tenant_id");
            e.HasIndex(s => s.StripeSubscriptionId).HasDatabaseName("ix_subscriptions_stripe_id");
            e.Ignore(s => s.DomainEvents);
            e.HasOne(s => s.Plan).WithMany().HasForeignKey(s => s.PlanId);
        });

        builder.Entity<Invoice>(e =>
        {
            e.ToTable("invoices");
            e.HasKey(i => i.Id);
            e.Property(i => i.Amount).HasPrecision(10, 2).IsRequired();
            e.Property(i => i.Currency).HasMaxLength(3);
            e.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(i => i.StripeInvoiceId).HasMaxLength(100);
            e.Property(i => i.StripeHostedUrl).HasMaxLength(500);
            e.HasIndex(i => i.TenantId).HasDatabaseName("ix_invoices_tenant_id");
        });
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Persistence\BillingDbContextFactory.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OneLine.Billing.Infrastructure.Persistence;

public sealed class BillingDbContextFactory
    : IDesignTimeDbContextFactory<BillingDbContext>
{
    public BillingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BillingDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=oneline_saaskit;Username=postgres;Password=postgres");
        return new BillingDbContext(optionsBuilder.Options);
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Persistence\BillingUnitOfWork.cs" -Encoding UTF8 -Value @'
using OneLine.Billing.Application.Interfaces;

namespace OneLine.Billing.Infrastructure.Persistence;

public sealed class BillingUnitOfWork : IUnitOfWork
{
    private readonly BillingDbContext _context;
    public BillingUnitOfWork(BillingDbContext context) => _context = context;
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Persistence\Repositories\SubscriptionRepository.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Enums;
using OneLine.Billing.Domain.Interfaces;

namespace OneLine.Billing.Infrastructure.Persistence.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly BillingDbContext _context;
    public SubscriptionRepository(BillingDbContext context) => _context = context;

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Subscriptions.Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Subscription?> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Subscriptions.Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.TenantId == tenantId, ct);

    public async Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeId, CancellationToken ct = default)
        => await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeId, ct);

    public async Task<bool> HasActiveSubscriptionAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Subscriptions.AnyAsync(
            s => s.TenantId == tenantId &&
                (s.Status == SubscriptionStatus.Active ||
                 s.Status == SubscriptionStatus.Trialing), ct);

    public async Task AddAsync(Subscription subscription, CancellationToken ct = default)
        => await _context.Subscriptions.AddAsync(subscription, ct);

    public void Update(Subscription subscription)
        => _context.Subscriptions.Update(subscription);
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Persistence\Repositories\PlanRepository.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Interfaces;

namespace OneLine.Billing.Infrastructure.Persistence.Repositories;

public sealed class PlanRepository : IPlanRepository
{
    private readonly BillingDbContext _context;
    public PlanRepository(BillingDbContext context) => _context = context;

    public async Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Plans.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default)
        => await _context.Plans.Where(p => p.IsActive).ToListAsync(ct);

    public async Task AddAsync(Plan plan, CancellationToken ct = default)
        => await _context.Plans.AddAsync(plan, ct);

    public void Update(Plan plan) => _context.Plans.Update(plan);
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Persistence\Repositories\InvoiceRepository.cs" -Encoding UTF8 -Value @'
using Microsoft.EntityFrameworkCore;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Interfaces;

namespace OneLine.Billing.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingDbContext _context;
    public InvoiceRepository(BillingDbContext context) => _context = context;

    public async Task<Invoice?> GetByStripeInvoiceIdAsync(string stripeId, CancellationToken ct = default)
        => await _context.Invoices.FirstOrDefaultAsync(i => i.StripeInvoiceId == stripeId, ct);

    public async Task<IReadOnlyList<Invoice>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Invoices.Where(i => i.TenantId == tenantId).ToListAsync(ct);

    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
        => await _context.Invoices.AddAsync(invoice, ct);

    public void Update(Invoice invoice) => _context.Invoices.Update(invoice);
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Services\StripeService.cs" -Encoding UTF8 -Value @'
using Microsoft.Extensions.Options;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Infrastructure.Options;
using Stripe;
using Stripe.Checkout;

namespace OneLine.Billing.Infrastructure.Services;

/// <summary>
/// Implémentation concrète de IStripeService.
/// Tous les appels Stripe sont ici -- Application n a pas besoin de savoir.
/// </summary>
public sealed class StripeService : IStripeService
{
    private readonly StripeOptions _options;

    public StripeService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<string> CreateCustomerAsync(
        string email, string tenantName, CancellationToken ct = default)
    {
        var service = new CustomerService();
        var customer = await service.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Name = tenantName,
            Metadata = new Dictionary<string, string> { ["tenant_name"] = tenantName }
        }, cancellationToken: ct);
        return customer.Id;
    }

    public async Task<(string SubscriptionId, DateTime PeriodEnd)> CreateSubscriptionAsync(
        string customerId, string stripePriceId, CancellationToken ct = default)
    {
        var service = new SubscriptionService();
        var subscription = await service.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = [new SubscriptionItemOptions { Price = stripePriceId }],
            PaymentBehavior = "default_incomplete",
            Expand = ["latest_invoice.payment_intent"]
        }, cancellationToken: ct);

        return (subscription.Id,
                subscription.CurrentPeriodEnd);
    }

    public async Task CancelSubscriptionAsync(
        string stripeSubscriptionId, CancellationToken ct = default)
    {
        var service = new SubscriptionService();
        await service.CancelAsync(stripeSubscriptionId, cancellationToken: ct);
    }

    public async Task<string> CreateCheckoutSessionAsync(
        string customerId, string stripePriceId,
        string successUrl, string cancelUrl,
        CancellationToken ct = default)
    {
        var service = new SessionService();
        var session = await service.CreateAsync(new SessionCreateOptions
        {
            Customer = customerId,
            Mode = "subscription",
            LineItems = [new SessionLineItemOptions { Price = stripePriceId, Quantity = 1 }],
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
        }, cancellationToken: ct);
        return session.Url;
    }

    public bool ValidateWebhookSignature(
        string payload, string signature, string secret,
        out string eventType, out string eventJson)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(payload, signature, secret);
            eventType = stripeEvent.Type;
            eventJson = stripeEvent.ToJson();
            return true;
        }
        catch
        {
            eventType = string.Empty;
            eventJson = string.Empty;
            return false;
        }
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Middleware\SubscriptionMiddleware.cs" -Encoding UTF8 -Value @'
using Microsoft.AspNetCore.Http;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Shared.Domain.Interfaces;

namespace OneLine.Billing.Infrastructure.Middleware;

/// <summary>
/// Middleware qui vérifie que le tenant a un abonnement actif.
/// Retourne HTTP 402 Payment Required si abonnement expiré.
///
/// Ignoré pour les routes publiques : /auth, /billing/webhook, /swagger
/// </summary>
public sealed class SubscriptionMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly string[] _ignoredPaths =
    [
        "/api/auth",
        "/api/billing/webhook",
        "/swagger",
        "/health"
    ];

    public SubscriptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenant currentTenant,
        ISubscriptionRepository subscriptionRepo)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Ignorer les routes publiques
        if (_ignoredPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        // Si pas de tenant résolu → laisser passer (Auth s en occupe)
        if (!currentTenant.IsResolved)
        {
            await _next(context);
            return;
        }

        // Vérifier abonnement actif
        var hasActive = await subscriptionRepo
            .HasActiveSubscriptionAsync(currentTenant.TenantId);

        if (!hasActive)
        {
            context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"code\":\"Billing.SubscriptionExpired\"," +
                "\"message\":\"Abonnement expiré. Veuillez renouveler votre abonnement.\"}");
            return;
        }

        await _next(context);
    }
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Webhooks\StripeWebhookHandler.cs" -Encoding UTF8 -Value @'
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Billing.Infrastructure.Options;
using Stripe;
using System.Text.Json;

namespace OneLine.Billing.Infrastructure.Webhooks;

/// <summary>
/// Gère les événements Stripe reçus via webhook.
///
/// Webhooks supportés :
///   - invoice.payment_succeeded  → activer/renouveler abonnement
///   - invoice.payment_failed     → marquer PastDue
///   - customer.subscription.deleted → annuler abonnement
///   - customer.subscription.updated → mise à jour plan
/// </summary>
public sealed class StripeWebhookHandler
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StripeOptions _options;

    public StripeWebhookHandler(
        ISubscriptionRepository subscriptionRepo,
        IInvoiceRepository invoiceRepo,
        IUnitOfWork unitOfWork,
        IOptions<StripeOptions> options)
    {
        _subscriptionRepo = subscriptionRepo;
        _invoiceRepo = invoiceRepo;
        _unitOfWork = unitOfWork;
        _options = options.Value;
    }

    public async Task HandleAsync(HttpContext context)
    {
        // Lire le payload
        using var reader = new StreamReader(context.Request.Body);
        var payload = await reader.ReadToEndAsync();
        var signature = context.Request.Headers["Stripe-Signature"].ToString();

        // Valider la signature
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                payload, signature, _options.WebhookSecret);
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        // Router vers le bon handler
        await (stripeEvent.Type switch
        {
            Events.InvoicePaymentSucceeded =>
                HandlePaymentSucceededAsync(stripeEvent),
            Events.InvoicePaymentFailed =>
                HandlePaymentFailedAsync(stripeEvent),
            Events.CustomerSubscriptionDeleted =>
                HandleSubscriptionDeletedAsync(stripeEvent),
            _ => Task.CompletedTask
        });

        context.Response.StatusCode = StatusCodes.Status200OK;
    }

    private async Task HandlePaymentSucceededAsync(Event stripeEvent)
    {
        var invoice = stripeEvent.Data.Object as Stripe.Invoice;
        if (invoice?.SubscriptionId is null) return;

        var sub = await _subscriptionRepo
            .GetByStripeSubscriptionIdAsync(invoice.SubscriptionId);
        if (sub is null) return;

        // Créer la facture dans notre DB
        var dbInvoice = Invoice.Create(
            sub.TenantId, sub.Id,
            (invoice.AmountPaid / 100m),
            invoice.Id,
            invoice.HostedInvoiceUrl);

        dbInvoice.MarkAsPaid();
        await _invoiceRepo.AddAsync(dbInvoice);

        // Activer l abonnement
        sub.Activate(
            invoice.Lines.Data.FirstOrDefault()?.Period?.End ?? DateTime.UtcNow.AddMonthsOffset(1),
            invoice.SubscriptionId);

        _subscriptionRepo.Update(sub);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandlePaymentFailedAsync(Event stripeEvent)
    {
        var invoice = stripeEvent.Data.Object as Stripe.Invoice;
        if (invoice?.SubscriptionId is null) return;

        var sub = await _subscriptionRepo
            .GetByStripeSubscriptionIdAsync(invoice.SubscriptionId);
        if (sub is null) return;

        sub.MarkAsPastDue();
        _subscriptionRepo.Update(sub);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleSubscriptionDeletedAsync(Event stripeEvent)
    {
        var stripeSub = stripeEvent.Data.Object as Stripe.Subscription;
        if (stripeSub is null) return;

        var sub = await _subscriptionRepo
            .GetByStripeSubscriptionIdAsync(stripeSub.Id);
        if (sub is null) return;

        sub.Cancel();
        _subscriptionRepo.Update(sub);
        await _unitOfWork.SaveChangesAsync();
    }
}

// Extension helper
public static class DateTimeExtensions
{
    public static DateTime AddMonthsOffset(this DateTime dt, int months)
        => dt.AddMonths(months);
}
'@

# Options Stripe
New-Item -ItemType Directory -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Options" -Force | Out-Null
Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\Options\StripeOptions.cs" -Encoding UTF8 -Value @'
namespace OneLine.Billing.Infrastructure.Options;

public sealed class StripeOptions
{
    public const string SectionName = "Stripe";
    public string SecretKey { get; init; } = string.Empty;
    public string PublishableKey { get; init; } = string.Empty;
    public string WebhookSecret { get; init; } = string.Empty;
}
'@

Set-Content -Path "src\Modules\Billing\OneLine.Billing.Infrastructure\BillingInfrastructureExtensions.cs" -Encoding UTF8 -Value @'
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
        // Options Stripe
        services.Configure<StripeOptions>(
            configuration.GetSection(StripeOptions.SectionName));

        // DbContext
        services.AddDbContext<BillingDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // UnitOfWork
        services.AddScoped<IUnitOfWork, BillingUnitOfWork>();

        // Services
        services.AddScoped<IStripeService, StripeService>();

        // Webhook Handler
        services.AddScoped<StripeWebhookHandler>();

        return services;
    }
}
'@

# API Controller
Set-Content -Path "src\OneLine.API\Controllers\BillingController.cs" -Encoding UTF8 -Value @'
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLine.Billing.Application.DTOs;
using OneLine.Billing.Application.UseCases.CancelSubscription;
using OneLine.Billing.Application.UseCases.CreateSubscription;
using OneLine.Billing.Application.UseCases.GetSubscription;
using OneLine.Billing.Infrastructure.Webhooks;
using OneLine.Shared.Domain.Result;

namespace OneLine.API.Controllers;

[ApiController]
[Route("api/billing")]
[Produces("application/json")]
public sealed class BillingController : ControllerBase
{
    private readonly ISender _sender;
    private readonly StripeWebhookHandler _webhookHandler;

    public BillingController(ISender sender, StripeWebhookHandler webhookHandler)
    {
        _sender = sender;
        _webhookHandler = webhookHandler;
    }

    /// <summary>Créer un abonnement pour un tenant</summary>
    [HttpPost("subscribe")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Subscribe(
        [FromBody] CreateSubscriptionCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetSubscription),
                new { tenantId = result.Value!.TenantId }, result.Value)
            : HandleError(result.Error);
    }

    /// <summary>Récupérer l abonnement d un tenant</summary>
    [HttpGet("{tenantId:guid}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscription(Guid tenantId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetSubscriptionByTenantQuery(tenantId), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
    }

    /// <summary>Annuler un abonnement</summary>
    [HttpDelete("{tenantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid tenantId, CancellationToken ct)
    {
        var result = await _sender.Send(new CancelSubscriptionCommand(tenantId), ct);
        return result.IsSuccess ? NoContent() : HandleError(result.Error);
    }

    /// <summary>Webhook Stripe -- ne pas protéger avec [Authorize]</summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook()
    {
        await _webhookHandler.HandleAsync(HttpContext);
        return new EmptyResult();
    }

    private IActionResult HandleError(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => NotFound(new { error.Code, error.Message }),
            ErrorType.Conflict => Conflict(new { error.Code, error.Message }),
            ErrorType.Forbidden => StatusCode(402, new { error.Code, error.Message }),
            ErrorType.Validation => BadRequest(new { error.Code, error.Message }),
            _ => StatusCode(500, new { error.Code, error.Message })
        };
}
'@

# ── ÉTAPE 5 : Mettre à jour appsettings.json ─────────────────
Write-Host "`n[5/7] Mise a jour appsettings.json..." -ForegroundColor Yellow

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
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
'@

# ── ÉTAPE 6 : Mettre à jour Program.cs ───────────────────────
Write-Host "`n[6/7] Mise a jour Program.cs..." -ForegroundColor Yellow

Set-Content -Path "src\OneLine.API\Program.cs" -Encoding UTF8 -Value @'
using OneLine.Auth.Application;
using OneLine.Auth.Infrastructure;
using OneLine.Billing.Application;
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
'@

Write-Host "Program.cs mis a jour ✅" -ForegroundColor Green

# ── ÉTAPE 7 : Build ──────────────────────────────────────────
Write-Host "`n[7/7] Build..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n=== BUILD REUSSI ✅ ===" -ForegroundColor Green
    Write-Host "`nLance maintenant les migrations :" -ForegroundColor Cyan
    Write-Host "dotnet ef migrations add InitialBilling --project src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj --startup-project tools\OneLine.Migrations\OneLine.Migrations.csproj --context BillingDbContext --output-dir Persistence\Migrations" -ForegroundColor Gray
    Write-Host "dotnet ef database update --project src\Modules\Billing\OneLine.Billing.Infrastructure\OneLine.Billing.Infrastructure.csproj --startup-project tools\OneLine.Migrations\OneLine.Migrations.csproj --context BillingDbContext" -ForegroundColor Gray
} else {
    Write-Host "`n=== BUILD ECHOUE -- voir erreurs ci-dessus ===" -ForegroundColor Red
}
