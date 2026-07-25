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
