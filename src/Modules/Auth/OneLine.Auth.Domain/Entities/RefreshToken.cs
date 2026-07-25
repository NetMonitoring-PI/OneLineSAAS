using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneLine.Shared.Domain.Primitives;

namespace OneLine.Auth.Domain.Entities;

/// <summary>
/// Token de rafraîchissement pour renouveler le JWT.
/// 
/// Pourquoi les refresh tokens ?
/// → JWT expire vite (15 min) pour la sécurité
/// → Sans refresh token : utilisateur déconnecté toutes les 15 min
/// → Avec refresh token : renouvellement silencieux
///
/// Cycle de vie :
///   1. Créé lors du login → stocké en DB
///   2. Envoyé au client (cookie httpOnly)
///   3. Client l'envoie pour obtenir un nouveau JWT
///   4. Révoqué après utilisation (rotation)
///   5. Remplacé par un nouveau token
/// </summary>
public sealed class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedReason { get; private set; }

    /// <summary>Token qui a remplacé celui-ci (après rotation)</summary>
    public string? ReplacedByToken { get; private set; }

    /// <summary>IP de la machine qui a créé ce token</summary>
    public string? CreatedByIp { get; private set; }

    // ── Propriété calculée ───────────────────────────────────
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    // ── Factory Method ───────────────────────────────────────
    public static RefreshToken Create(
        Guid userId,
        string? createdByIp = null,
        int expiryDays = 7)
    {
        return new RefreshToken
        {
            // Génère un token aléatoire cryptographiquement sûr
            Token = GenerateSecureToken(),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            IsRevoked = false,
            CreatedByIp = createdByIp
        };
    }

    /// <summary>
    /// Révoquer ce token.
    /// Appelé lors du logout ou si une anomalie est détectée.
    /// </summary>
    public void Revoke(string reason, string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
        ReplacedByToken = replacedByToken;
    }

    private static string GenerateSecureToken()
    {
        // 64 bytes = 512 bits → très difficile à deviner
        var randomBytes = new byte[64];
        System.Security.Cryptography.RandomNumberGenerator
            .Fill(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
