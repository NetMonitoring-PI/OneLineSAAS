using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneLine.Shared.Domain.Result;

namespace OneLine.Auth.Domain.Errors;

/// <summary>
/// Toutes les erreurs possibles du module Auth.
/// 
/// Centralisées ici → facile à maintenir et à tester.
/// Au lieu de : Error.NotFound("Auth.001", "User not found")
/// partout dans le code, on écrit : AuthErrors.UserNotFound
/// 
/// Pattern : Static Error Definitions
/// </summary>
public static class AuthErrors
{
    // ── Erreurs utilisateur ──────────────────────────────────
    public static readonly Error UserNotFound =
        Error.NotFound(
            "Auth.UserNotFound",
            "L'utilisateur n'existe pas.");

    public static readonly Error InvalidCredentials =
        Error.Unauthorized(
            "Auth.InvalidCredentials",
            "Email ou mot de passe incorrect.");

    public static readonly Error EmailAlreadyExists =
        Error.Conflict(
            "Auth.EmailAlreadyExists",
            "Un compte avec cet email existe déjà.");

    public static readonly Error UserNotActive =
        Error.Forbidden(
            "Auth.UserNotActive",
            "Ce compte est désactivé.");

    // ── Erreurs token ────────────────────────────────────────
    public static readonly Error InvalidToken =
        Error.Unauthorized(
            "Auth.InvalidToken",
            "Le token est invalide ou expiré.");

    public static readonly Error TokenExpired =
        Error.Unauthorized(
            "Auth.TokenExpired",
            "Le token a expiré.");

    public static readonly Error RefreshTokenNotFound =
        Error.NotFound(
            "Auth.RefreshTokenNotFound",
            "Le refresh token n'existe pas.");

    public static readonly Error RefreshTokenRevoked =
        Error.Unauthorized(
            "Auth.RefreshTokenRevoked",
            "Le refresh token a été révoqué.");
}