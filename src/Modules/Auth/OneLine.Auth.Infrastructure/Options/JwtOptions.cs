using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Auth.Infrastructure.Options;

/// <summary>
/// Configuration JWT lue depuis appsettings.json.
///
/// Pattern : Options Pattern
/// → Configuration fortement typée
/// → Plus sûr que lire des strings directement
/// → Validée au démarrage de l'application
/// </summary>
public sealed class JwtOptions
{
    // Nom de la section dans appsettings.json
    public const string SectionName = "Jwt";

    /// <summary>Clé secrète pour signer les tokens — min 32 chars</summary>
    public string SecretKey { get; init; } = string.Empty;

    /// <summary>Émetteur du token (ton API)</summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>Destinataire du token (ton frontend)</summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>Durée de vie du JWT en minutes (défaut: 15)</summary>
    public int AccessTokenExpiryMinutes { get; init; } = 15;

    /// <summary>Durée de vie du refresh token en jours (défaut: 7)</summary>
    public int RefreshTokenExpiryDays { get; init; } = 7;
}
