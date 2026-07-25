using MediatR;
using OneLine.Shared.Domain.Result;
using OneLine.Tenants.Application.DTOs;
using OneLine.Tenants.Application.Interfaces;
using OneLine.Tenants.Domain.Entities;
using OneLine.Tenants.Domain.Errors;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Application.UseCases.CreateTenant;

public sealed class CreateTenantCommandHandler
    : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    private readonly ITenantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantCommandHandler(
        ITenantRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TenantDto>> Handle(
        CreateTenantCommand command, CancellationToken ct)
    {
        var exists = await _repository.ExistsBySubdomainAsync(command.Subdomain, ct);
        if (exists) return TenantErrors.SubdomainAlreadyExists;

        var tenant = Tenant.Create(command.Name, command.Subdomain,
            command.ContactEmail, command.Plan);

        await _repository.AddAsync(tenant, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new TenantDto(tenant.Id, tenant.Name, tenant.Subdomain,
            tenant.Plan.ToString(), tenant.Status.ToString(),
            tenant.ContactEmail, tenant.IsActive,
            tenant.TrialEndsAt, tenant.CreatedAt);
    }
}
