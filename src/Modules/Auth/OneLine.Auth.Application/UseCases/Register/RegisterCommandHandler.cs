using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using Microsoft.AspNetCore.Identity;
using OneLine.Auth.Application.DTOs;
using OneLine.Auth.Application.Interfaces;
using OneLine.Auth.Domain.Entities;
using OneLine.Auth.Domain.Errors;
using OneLine.Auth.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.Auth.Application.UseCases.Register;

/// <summary>
/// Handler qui exécute la logique d'inscription.
///
/// Reçoit RegisterCommand → retourne Result<TokenResponse>
///
/// Étapes :
///   1. Vérifier que l'email n'existe pas
///   2. Créer l'entité AppUser
///   3. Hasher le mot de passe (via Identity)
///   4. Sauvegarder en DB
///   5. Générer les tokens
///   6. Retourner la réponse
/// </summary>
public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    // Injection de dépendances → constructeur
    // Toutes les dépendances sont des INTERFACES (pas des classes)
    // → Principe D de SOLID respecté
    public RegisterCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<Result<TokenResponse>> Handle(
        RegisterCommand command,
        CancellationToken ct)
    {
        // ── Étape 1 : Email déjà utilisé ? ──────────────────
        var emailExists = await _userRepository
            .ExistsByEmailAsync(command.Email, ct);

        if (emailExists)
            return AuthErrors.EmailAlreadyExists;
        //  ↑ conversion implicite Error → Result<TokenResponse>

        // ── Étape 2 : Créer l'entité via Factory Method ─────
        var user = AppUser.Create(
            command.FirstName,
            command.LastName,
            command.Email,
            command.TenantId,
            command.Role);

        // ── Étape 3 : Hasher le mot de passe ────────────────
        // UserManager.CreateAsync hash automatiquement le password
        var createResult = await _userManager.CreateAsync(
            user,
            command.Password);

        if (!createResult.Succeeded)
        {
            var errorMessage = string.Join(", ",
                createResult.Errors.Select(e => e.Description));

            return Error.Failure("Auth.CreateFailed", errorMessage);
        }

        // ── Étape 4 : Générer les tokens ─────────────────────
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        // ── Étape 5 : Sauvegarder le refresh token ───────────
        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // ── Étape 6 : Retourner la réponse ───────────────────
        return new TokenResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token,
            AccessTokenExpiresAt: DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt: refreshToken.ExpiresAt,
            UserId: user.Id.ToString(),
            Email: user.Email!,
            FullName: user.FullName,
            Role: user.Role.ToString()
        );
    }
}
