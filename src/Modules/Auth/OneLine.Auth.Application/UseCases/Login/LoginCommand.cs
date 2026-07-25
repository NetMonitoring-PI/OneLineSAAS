using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using OneLine.Auth.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.Auth.Application.UseCases.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    string? IpAddress = null
) : IRequest<Result<TokenResponse>>;
