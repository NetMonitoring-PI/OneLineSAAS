using MediatR;

namespace OneLine.Tenants.Domain.Events;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
