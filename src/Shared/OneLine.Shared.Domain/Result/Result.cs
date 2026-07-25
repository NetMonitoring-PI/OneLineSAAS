using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Shared.Domain.Result;

/// <summary>
/// Pattern Result — alternative aux exceptions pour les erreurs métier.
/// 
/// Problème des exceptions :
///   throw new Exception("User not found")
///   → coûteux en performance
///   → oblige à utiliser try/catch partout
///   → pas explicite dans la signature de la méthode
/// 
/// Avec Result<T> :
///   Result<User> result = await GetUserAsync(id);
///   if (result.IsFailure) return result.Error;
///   → Explicite, performant, lisible
/// 
/// Pattern : Result Pattern (aussi appelé Railway Oriented Programming)
/// Utilisé par : Rust, F#, et de plus en plus en C#
/// </summary>
public class Result<T>
{
    // Les propriétés sont en lecture seule après création
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error Error { get; }

    // Constructeur privé → on force l'utilisation de
    // Success() et Failure() pour créer un Result
    // C'est le pattern Factory Method
    private Result(T? value, Error error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    // ── Méthodes statiques de fabrique ──────────────────────

    /// <summary>Crée un résultat de succès avec une valeur</summary>
    public static Result<T> Success(T value)
        => new(value, Error.None, true);

    /// <summary>Crée un résultat d'échec avec une erreur</summary>
    public static Result<T> Failure(Error error)
        => new(default, error, false);

    // ── Opérateurs implicites ────────────────────────────────
    // Permettent la conversion automatique :
    // return user;       → converti en Result<User>.Success(user)
    // return someError;  → converti en Result<User>.Failure(error)

    public static implicit operator Result<T>(T value)
        => Success(value);

    public static implicit operator Result<T>(Error error)
        => Failure(error);
}

/// <summary>
/// Version sans valeur de retour — pour les opérations void
/// Exemple : DeleteUser() ne retourne rien mais peut échouer
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    private Result(Error error, bool isSuccess)
    {
        Error = error;
        IsSuccess = isSuccess;
    }

    public static Result Success()
        => new(Error.None, true);

    public static Result Failure(Error error)
        => new(error, false);

    public static implicit operator Result(Error error)
        => Failure(error);
}
