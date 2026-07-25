using OneLine.Shared.Domain.Result;

namespace OneLine.Tenants.Domain.Errors;

public static class TenantErrors
{
    public static readonly Error TenantNotFound =
        Error.NotFound("Tenant.NotFound", "Le tenant n'existe pas.");

    public static readonly Error SubdomainAlreadyExists =
        Error.Conflict("Tenant.SubdomainExists", "Ce sous-domaine est déjà utilisé.");

    public static readonly Error TenantNotActive =
        Error.Forbidden("Tenant.NotActive", "Ce tenant est suspendu ou annulé.");

    public static readonly Error TenantNotResolved =
        Error.Unauthorized("Tenant.NotResolved", "Impossible de déterminer le tenant.");
}
