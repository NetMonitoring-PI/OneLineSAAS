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
        CreateTenantCommand command,
        CancellationToken ct)
    {
        // 1. Vérifier unicité du subdomain
        var exists = await _repository
            .ExistsBySubdomainAsync(command.Subdomain, ct);

        if (exists)
            return TenantErrors.SubdomainAlreadyExists;

        // 2. Créer le tenant via Factory Method
        var tenant = Tenant.Create(
            command.Name,
            command.Subdomain,
            command.ContactEmail,
            command.Plan);

        // 3. Sauvegarder
        await _repository.AddAsync(tenant, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // 4. Retourner DTO
        return ToDto(tenant);
    }

    private static TenantDto ToDto(Tenant t) => new(
        t.Id, t.Name, t.Subdomain,
        t.Plan.ToString(), t.Status.ToString(),
        t.ContactEmail, t.IsActive,
        t.TrialEndsAt, t.CreatedAt);
}
