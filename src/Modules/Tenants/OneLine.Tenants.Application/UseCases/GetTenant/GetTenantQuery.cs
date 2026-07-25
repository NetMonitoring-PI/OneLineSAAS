using MediatR;
using OneLine.Shared.Domain.Result;
using OneLine.Tenants.Application.DTOs;

namespace OneLine.Tenants.Application.UseCases.GetTenant;

public sealed record GetTenantQuery(Guid TenantId)
    : IRequest<Result<TenantDto>>;
