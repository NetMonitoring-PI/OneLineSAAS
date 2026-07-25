using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using OneLine.Auth.Application.DTOs;
using OneLine.Auth.Domain.Enums;
using OneLine.Shared.Domain.Result;

namespace OneLine.Auth.Application.UseCases.Register;

/// <summary>
/// Commande pour créer un nouveau compte utilisateur.
///
/// record → immuable, comparaison par valeur
/// IRequest<Result<TokenResponse>> → MediatR sait quel handler appeler
///                                    et quel type est retourné
/// </summary>
public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    Guid TenantId,
    UserRole Role = UserRole.User
) : IRequest<Result<TokenResponse>>;
