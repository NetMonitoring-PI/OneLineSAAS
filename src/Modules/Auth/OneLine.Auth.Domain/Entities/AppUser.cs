using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using OneLine.Auth.Domain.Enums;
using OneLine.Auth.Domain.Events;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.Auth.Domain.Entities;

/// <summary>
/// Entité utilisateur principale du système.
/// 
/// Hérite de IdentityUser → donne gratuitement :
///   - Email, PasswordHash, PhoneNumber
///   - LockoutEnabled, AccessFailedCount
///   - EmailConfirmed, TwoFactorEnabled
///
/// Hérite de BaseEntity → donne :
///   - Id (Guid), CreatedAt, UpdatedAt
///
/// Pattern : Rich Domain Model
/// → Les règles métier sont DANS l'entité
/// → Pas dans un service externe
/// → Ex : Activate(), Deactivate(), AssignRole()
/// </summary>
public sealed class AppUser : IdentityUser<Guid>
{
    // ── Propriétés métier ────────────────────────────────────

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    /// <summary>Tenant auquel appartient cet utilisateur</summary>
    public Guid TenantId { get; private set; }

    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>Tokens de rafraîchissement actifs</summary>
    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens;

    // ── Constructeur privé ───────────────────────────────────
    // Pourquoi privé ?
    // → On force l'utilisation de Create()
    // → Impossible de créer un AppUser invalide
    // → Toute création passe par les règles métier
    private AppUser() { }

    // ── Factory Method (Design Pattern) ─────────────────────
    /// <summary>
    /// Seule façon de créer un utilisateur valide.
    /// Garantit que toutes les règles sont respectées.
    /// </summary>
    public static AppUser Create(
        string firstName,
        string lastName,
        string email,
        Guid tenantId,
        UserRole role = UserRole.User)
    {
        // Validation des règles métier
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,        // Identity utilise UserName
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            TenantId = tenantId,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        // On enregistre l'événement domaine
        // → autres parties du système peuvent réagir
        user._domainEvents.Add(new UserCreatedEvent(user.Id, tenantId));

        return user;
    }

    // ── Comportements métier (Rich Domain Model) ─────────────

    /// <summary>Activer le compte utilisateur</summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Désactiver le compte utilisateur</summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Changer le rôle de l'utilisateur</summary>
    public void AssignRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Mettre à jour le profil</summary>
    public void UpdateProfile(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Ajouter un refresh token</summary>
    public void AddRefreshToken(RefreshToken token)
    {
        _refreshTokens.Add(token);
    }

    /// <summary>Révoquer tous les refresh tokens actifs</summary>
    public void RevokeAllTokens(string reason)
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
        {
            token.Revoke(reason);
        }
    }

    // ── Propriété calculée ───────────────────────────────────
    public string FullName => $"{FirstName} {LastName}";

    // ── Domain Events ────────────────────────────────────────
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    public void ClearDomainEvents() => _domainEvents.Clear();
}
