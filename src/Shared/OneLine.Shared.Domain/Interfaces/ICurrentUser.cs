using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Shared.Domain.Interfaces;

/// <summary>
/// Contrat pour accéder à l'utilisateur connecté.
/// 
/// Pourquoi une interface ?
/// → Principe D de SOLID (Dependency Inversion)
/// → Le code métier ne dépend pas de HttpContext directement
/// → Testable : en test on injecte un FakeCurrentUser
/// → En production on injecte HttpContextCurrentUser
/// </summary>
public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }

    bool IsInRole(string role);
}