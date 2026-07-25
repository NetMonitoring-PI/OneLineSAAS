using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Auth.Application.DTOs;

/// <summary>
/// Réponse retournée après login ou refresh.
/// Contient les deux tokens nécessaires.
///
/// AccessToken  → JWT de courte durée (15 min)
///               Envoyé dans le header Authorization
///
/// RefreshToken → Token longue durée (7 jours)
///               Stocké en cookie HttpOnly côté client
///               Jamais accessible en JavaScript (sécurité)
/// </summary>
public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    string UserId,
    string Email,
    string FullName,
    string Role
);
