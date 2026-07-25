using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneLine.Auth.Domain.Entities;

namespace OneLine.Auth.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken ct = default);

    Task AddAsync(RefreshToken token, CancellationToken ct = default);

    void Update(RefreshToken token);

    /// <summary>Supprimer les tokens expirés (nettoyage)</summary>
    Task DeleteExpiredTokensAsync(CancellationToken ct = default);
}