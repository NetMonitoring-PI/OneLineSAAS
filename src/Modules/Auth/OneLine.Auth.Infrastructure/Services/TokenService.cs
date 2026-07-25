using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OneLine.Auth.Application.Interfaces;
using OneLine.Auth.Domain.Entities;
using OneLine.Auth.Infrastructure.Options;

namespace OneLine.Auth.Infrastructure.Services;

/// <summary>
/// Implémentation concrète de ITokenService.
/// Génère et valide les tokens JWT.
///
/// JWT = JSON Web Token
/// Structure : Header.Payload.Signature
///
/// Header   → algorithme (HS256)
/// Payload  → claims (UserId, Email, Role, TenantId...)
/// Signature → garantit que le token n'a pas été modifié
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateAccessToken(AppUser user)
    {
        // Clé de signature
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Claims = données encodées dans le token
        // Le client peut les lire, pas les modifier
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("tenant_id", user.TenantId.ToString()),
            new("role", user.Role.ToString()),
            new("first_name", user.FirstName),
            new("last_name", user.LastName),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _options.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(
        Guid userId,
        string? ipAddress = null)
    {
        return RefreshToken.Create(
            userId,
            ipAddress,
            _options.RefreshTokenExpiryDays);
    }

    public Guid? ValidateAccessToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_options.SecretKey);

            tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = false,
                    // false → on valide même les tokens expirés
                    // pour le endpoint /refresh
                }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims
                .First(c => c.Type == JwtRegisteredClaimNames.Sub)
                .Value;

            return Guid.Parse(userId);
        }
        catch
        {
            // Token invalide → null
            return null;
        }
    }
}