using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneLine.Auth.Domain.Entities;

namespace OneLine.Auth.Application.Interfaces;

/// <summary>
/// Contrat pour la génération et validation des tokens JWT.
///
/// Pourquoi une interface ici dans Application ?
/// → Application a BESOIN de tokens mais ne sait pas
///   comment les générer (c'est Infrastructure qui sait)
/// → Principe D de SOLID : dépendre des abstractions
/// → En test : on injecte un FakeTokenService
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Génère un JWT access token pour l'utilisateur.
    /// </summary>
    string GenerateAccessToken(AppUser user);

    /// <summary>
    /// Génère un refresh token sécurisé.
    /// </summary>
    Domain.Entities.RefreshToken GenerateRefreshToken(
        Guid userId,
        string? ipAddress = null);

    /// <summary>
    /// Valide un access token et retourne le UserId.
    /// Utilisé pour le endpoint /refresh.
    /// </summary>
    Guid? ValidateAccessToken(string token);
}