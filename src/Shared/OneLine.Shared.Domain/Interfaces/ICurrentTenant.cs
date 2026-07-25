using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Shared.Domain.Interfaces;

/// <summary>
/// Contrat pour accéder au tenant courant.
/// 
/// Injecté dans tous les services qui ont besoin
/// de savoir QUEL tenant fait la requête.
/// 
/// L'implémentation lit depuis HttpContext
/// (header, subdomain ou JWT claim selon config)
/// </summary>
public interface ICurrentTenant
{
    Guid TenantId { get; }
    string TenantName { get; }
    string? Plan { get; }
    bool IsResolved { get; }
}
