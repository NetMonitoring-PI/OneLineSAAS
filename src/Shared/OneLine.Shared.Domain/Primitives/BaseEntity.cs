using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Shared.Domain.Primitives;

/// <summary>
/// Classe de base pour toutes les entités du système.
/// Chaque entité a un Id unique, une date de création et de mise à jour.
/// 
/// Pattern : Entity Base Class
/// Pourquoi : éviter de répéter Id, CreatedAt, UpdatedAt
///            dans chaque entité (Auth, Tenants, Billing...)
/// </summary>
public abstract class BaseEntity
{
    // Guid = identifiant unique universel
    // Pas d'int auto-increment → meilleur pour multi-tenant
    // et systèmes distribués (pas de collision d'IDs)
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; private set; }

    // Constructeur protégé → seules les classes filles peuvent
    // créer une entité. On force l'utilisation de méthodes
    // statiques de fabrique (Factory Method pattern)
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    // Appelé quand l'entité est modifiée
    // protected → seule la classe fille peut l'appeler
    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
