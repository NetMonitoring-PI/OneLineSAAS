using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Shared.Domain.Pagination;

/// <summary>
/// Paramètres de pagination pour toutes les requêtes de liste.
/// Standardise la pagination dans tout le système.
/// </summary>
public sealed class PaginationParams
{
    // Constantes → valeurs par défaut et limites
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private int _pageSize = DefaultPageSize;

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize
            ? MaxPageSize  // on limite à 100 pour protéger la DB
            : value;
    }

    // Calcul de l'offset pour EF Core
    // Page 1 → Skip(0)
    // Page 2 → Skip(10)
    // Page 3 → Skip(20)
    public int Skip => (Page - 1) * PageSize;
}