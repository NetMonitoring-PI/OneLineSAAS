using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneLine.Auth.Domain.Entities;
using OneLine.Shared.Domain.Result;

namespace OneLine.Auth.Domain.Interfaces;

/// <summary>
/// Contrat pour l'accès aux données utilisateur.
/// 
/// Pattern : Repository
/// → Domain définit le CONTRAT (cette interface)
/// → Infrastructure fournit l'IMPLÉMENTATION (EF Core)
/// → Application utilise le CONTRAT, pas l'implémentation
///
/// Avantage :
/// → Tester Application sans base de données réelle
/// → Changer PostgreSQL pour SQL Server sans toucher au domaine
/// </summary>
public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<AppUser?> GetByIdWithTokensAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

    Task AddAsync(AppUser user, CancellationToken ct = default);

    void Update(AppUser user);

    void Delete(AppUser user);
}
