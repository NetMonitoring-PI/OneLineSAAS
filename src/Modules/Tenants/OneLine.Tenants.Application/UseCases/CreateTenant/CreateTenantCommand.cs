using MediatR;
using OneLine.Shared.Domain.Result;
using OneLine.Tenants.Application.DTOs;
using OneLine.Tenants.Domain.Enums;

namespace OneLine.Tenants.Application.UseCases.CreateTenant;

public sealed record CreateTenantCommand(
    string Name,
    string Subdomain,
    string? ContactEmail = null,
    TenantPlan Plan = TenantPlan.Free
) : IRequest<Result<TenantDto>>;
