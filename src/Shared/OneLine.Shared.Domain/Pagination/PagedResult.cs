using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Shared.Domain.Pagination;

/// <summary>
/// Résultat paginé standard pour toutes les listes.
/// Retourné par tous les endpoints de liste du système.
/// </summary>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }

    // Calculés automatiquement
    public int TotalPages => (int)Math.Ceiling(
        (double)TotalCount / PageSize);

    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedResult(
        IReadOnlyList<T> items,
        int totalCount,
        int page,
        int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    // Factory method — plus lisible que le constructeur
    public static PagedResult<T> Create(
        IReadOnlyList<T> items,
        int totalCount,
        PaginationParams pagination)
        => new(items, totalCount, pagination.Page, pagination.PageSize);
}