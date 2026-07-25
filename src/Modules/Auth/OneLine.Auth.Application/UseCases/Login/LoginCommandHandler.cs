using MediatR;
using Microsoft.AspNetCore.Identity;
using OneLine.Auth.Application.DTOs;
using OneLine.Auth.Application.Interfaces;
using OneLine.Auth.Domain.Entities;
using OneLine.Auth.Domain.Errors;
using OneLine.Auth.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.Auth.Application.UseCases.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public LoginCommandHandler(
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
        LoginCommand command,
        CancellationToken ct)
    {
        var user = await _userRepository
            .GetByEmailAsync(command.Email, ct);

        if (user is null)
            return AuthErrors.InvalidCredentials;

        if (!user.IsActive)
            return AuthErrors.UserNotActive;

        var passwordValid = await _userManager
            .CheckPasswordAsync(user, command.Password);

        if (!passwordValid)
            return AuthErrors.InvalidCredentials;

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(
            user.Id, command.IpAddress);

        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        await _unitOfWork.SaveChangesAsync(ct);

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
