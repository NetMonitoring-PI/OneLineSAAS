using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace OneLine.Auth.Domain.Events;

/// <summary>
/// Marqueur pour tous les événements domaine.
/// 
/// Domain Events = "quelque chose s'est passé dans le domaine"
/// 
/// Exemples :
///   UserCreatedEvent → envoyer email de bienvenue
///   UserDeactivatedEvent → révoquer tous ses tokens
///   PasswordChangedEvent → notifier l'utilisateur
///
/// Pattern : Domain Events + Mediator
/// MediatR distribue l'événement aux handlers intéressés
/// → couplage faible entre les modules
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
