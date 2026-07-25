namespace OneLine.Tenants.Domain.Events;

public sealed record TenantCreatedEvent(
    Guid TenantId,
    string TenantName) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
