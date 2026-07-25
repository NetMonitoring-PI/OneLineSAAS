using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Auth.Domain.Events;

/// <summary>
/// Événement déclenché quand un nouvel utilisateur est créé.
/// 
/// Qui peut réagir à cet événement ?
/// → Module Email : envoyer email de bienvenue
/// → Module Billing : créer le profil billing
/// → Module Audit : logger la création
/// 
/// Chaque handler est indépendant et découplé.
/// </summary>
public sealed record UserCreatedEvent(
    Guid UserId,
    Guid TenantId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}