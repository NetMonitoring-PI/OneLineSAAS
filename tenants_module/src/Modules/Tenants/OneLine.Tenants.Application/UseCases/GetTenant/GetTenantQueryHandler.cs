using MediatR;
using OneLine.Shared.Domain.Result;
using OneLine.Tenants.Application.DTOs;
using OneLine.Tenants.Domain.Errors;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Application.UseCases.GetTenant;

public sealed class GetTenantQueryHandler
    : IRequestHandler<GetTenantQuery, Result<TenantDto>>
{
    private readonly ITenantRepository _repository;

    public GetTenantQueryHandler(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TenantDto>> Handle(
        GetTenantQuery query,
        CancellationToken ct)
    {
        var tenant = await _repository.GetByIdAsync(query.TenantId, ct);

        if (tenant is null)
            return TenantErrors.TenantNotFound;

        return new TenantDto(
            tenant.Id, tenant.Name, tenant.Subdomain,
            tenant.Plan.ToString(), tenant.Status.ToString(),
            tenant.ContactEmail, tenant.IsActive,
            tenant.TrialEndsAt, tenant.CreatedAt);
    }
}
