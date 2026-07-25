using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Shared.Domain.Result;

/// <summary>
/// Représente une erreur structurée dans le système.
/// 
/// Pourquoi : au lieu de lancer des exceptions pour les
/// erreurs métier (mauvais mot de passe, tenant inexistant...),
/// on retourne un objet Error explicite.
/// 
/// Pattern : Value Object
/// </summary>
public sealed record Error
{
    // "sealed record" → immuable, comparable par valeur
    // Deux Error avec le même Code sont identiques

    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    // ── Méthodes statiques de fabrique ──────────────────────
    // Au lieu de : new Error("Auth.NotFound", "...", ErrorType.NotFound)
    // On écrit  : Error.NotFound("Auth.NotFound", "User not found")
    // Plus lisible, plus expressif

    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);

    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    public static Error Failure(string code, string message)
        => new(code, message, ErrorType.Failure);

    // Erreur vide — quand il n'y a pas d'erreur
    public static readonly Error None = new(
        string.Empty,
        string.Empty,
        ErrorType.None);

    public override string ToString() => $"[{Code}] {Message}";
}

/// <summary>
/// Types d'erreurs — correspondent aux codes HTTP
/// </summary>
public enum ErrorType
{
    None = 0,
    NotFound = 1,       // 404
    Validation = 2,     // 400
    Unauthorized = 3,   // 401
    Forbidden = 4,      // 403
    Conflict = 5,       // 409
    Failure = 6         // 500
}
