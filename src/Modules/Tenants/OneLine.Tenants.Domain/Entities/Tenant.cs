using OneLine.Shared.Domain.Primitives;
using OneLine.Tenants.Domain.Enums;
using OneLine.Tenants.Domain.Events;

namespace OneLine.Tenants.Domain.Entities;

public sealed class Tenant : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Subdomain { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TenantPlan Plan { get; private set; }
    public TenantStatus Status { get; private set; }
    public string? ContactEmail { get; private set; }
    public DateTime? TrialEndsAt { get; private set; }
    public bool IsActive => Status == TenantStatus.Active || Status == TenantStatus.Trial;

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    public void ClearDomainEvents() => _domainEvents.Clear();

    private Tenant() { }

    public static Tenant Create(
        string name,
        string subdomain,
        string? contactEmail = null,
        TenantPlan plan = TenantPlan.Free)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(subdomain);

        var tenant = new Tenant
        {
            Name = name.Trim(),
            Subdomain = subdomain.ToLowerInvariant().Trim(),
            ContactEmail = contactEmail,
            Plan = plan,
            Status = TenantStatus.Trial,
            TrialEndsAt = DateTime.UtcNow.AddDays(14)
        };

        tenant._domainEvents.Add(new TenantCreatedEvent(tenant.Id, tenant.Name));
        return tenant;
    }

    public void Activate() { Status = TenantStatus.Active; SetUpdatedAt(); }
    public void Suspend() { Status = TenantStatus.Suspended; SetUpdatedAt(); }
    public void Upgrade(TenantPlan newPlan) { Plan = newPlan; SetUpdatedAt(); }

    public void UpdateInfo(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Description = description;
        SetUpdatedAt();
    }
}
