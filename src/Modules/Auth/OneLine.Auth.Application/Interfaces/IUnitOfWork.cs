using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Auth.Application.Interfaces;

/// <summary>
/// Pattern Unit of Work — encapsule les transactions.
///
/// Problème sans Unit of Work :
///   1. Créer utilisateur → SaveChanges()
///   2. Créer refresh token → SaveChanges()
///   → Si étape 2 échoue, l'utilisateur existe sans token
///   → Données incohérentes
///
/// Avec Unit of Work :
///   1. Créer utilisateur
///   2. Créer refresh token
///   3. SaveChangesAsync() → tout ou rien (transaction)
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
