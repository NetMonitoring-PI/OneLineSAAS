# ============================================================
# Script d'installation du Module Tenants
# Exécuter depuis la racine : C:\Users\DELL\Projects\OneLine.SaasKit
# ============================================================

Write-Host "=== Installation Module Tenants ===" -ForegroundColor Cyan

# ── 1. Installer packages NuGet ──────────────────────────────
Write-Host "`n[1/6] Installation packages NuGet..." -ForegroundColor Yellow

dotnet add src\Modules\Tenants\OneLine.Tenants.Domain\OneLine.Tenants.Domain.csproj `
  package MediatR --version 12.2.0

dotnet add src\Modules\Tenants\OneLine.Tenants.Application\OneLine.Tenants.Application.csproj `
  package MediatR --version 12.2.0

dotnet add src\Modules\Tenants\OneLine.Tenants.Application\OneLine.Tenants.Application.csproj `
  package FluentValidation --version 11.9.0

dotnet add src\Modules\Tenants\OneLine.Tenants.Application\OneLine.Tenants.Application.csproj `
  package FluentValidation.DependencyInjectionExtensions --version 11.9.0

dotnet add src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj `
  package Microsoft.EntityFrameworkCore --version 9.0.0

dotnet add src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj `
  package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0

dotnet add src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj `
  package Microsoft.EntityFrameworkCore.Design --version 9.0.0

dotnet add src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj `
  package Microsoft.AspNetCore.Http.Abstractions --version 2.2.0

# ── 2. Références entre projets ──────────────────────────────
Write-Host "`n[2/6] Configuration références..." -ForegroundColor Yellow

dotnet add src\Modules\Tenants\OneLine.Tenants.Domain\OneLine.Tenants.Domain.csproj `
  reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj

dotnet add src\Modules\Tenants\OneLine.Tenants.Application\OneLine.Tenants.Application.csproj `
  reference src\Modules\Tenants\OneLine.Tenants.Domain\OneLine.Tenants.Domain.csproj

dotnet add src\Modules\Tenants\OneLine.Tenants.Application\OneLine.Tenants.Application.csproj `
  reference src\Modules\Auth\OneLine.Auth.Application\OneLine.Auth.Application.csproj

dotnet add src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj `
  reference src\Modules\Tenants\OneLine.Tenants.Application\OneLine.Tenants.Application.csproj

dotnet add src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj `
  reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj

dotnet add src\OneLine.API\OneLine.API.csproj `
  reference src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj

# ── 3. Copier les fichiers source ────────────────────────────
Write-Host "`n[3/6] Copie des fichiers source..." -ForegroundColor Yellow

# Domain
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Domain\Enums\TenantPlan.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Domain\Enums\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Domain\Entities\Tenant.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Domain\Entities\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Domain\Events\IDomainEvent.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Domain\Events\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Domain\Events\TenantCreatedEvent.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Domain\Events\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Domain\Errors\TenantErrors.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Domain\Errors\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Domain\Interfaces\ITenantRepository.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Domain\Interfaces\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Domain\Interfaces\ITenantResolver.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Domain\Interfaces\"

# Application
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Application\DTOs\TenantDto.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Application\DTOs\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Application\Interfaces\IUnitOfWork.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Application\Interfaces\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Application\UseCases\CreateTenant\CreateTenantCommand.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Application\UseCases\CreateTenant\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Application\UseCases\CreateTenant\CreateTenantCommandValidator.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Application\UseCases\CreateTenant\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Application\UseCases\CreateTenant\CreateTenantCommandHandler.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Application\UseCases\CreateTenant\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Application\UseCases\GetTenant\GetTenantQuery.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Application\UseCases\GetTenant\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Application\UseCases\GetTenant\GetTenantQueryHandler.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Application\UseCases\GetTenant\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Application\TenantsApplicationExtensions.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Application\"

# Infrastructure
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Persistence\TenantsDbContext.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Persistence\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Persistence\TenantsDbContextFactory.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Persistence\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Persistence\TenantsUnitOfWork.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Persistence\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Persistence\Repositories\TenantRepository.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Persistence\Repositories\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Resolvers\HeaderTenantResolver.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Resolvers\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Resolvers\ClaimTenantResolver.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Resolvers\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Resolvers\SubdomainTenantResolver.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Resolvers\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Middleware\TenantMiddleware.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Middleware\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\Services\CurrentTenantService.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\Services\"
Copy-Item "tenants_module\src\Modules\Tenants\OneLine.Tenants.Infrastructure\TenantsInfrastructureExtensions.cs" `
  "src\Modules\Tenants\OneLine.Tenants.Infrastructure\"

# API Controller
Copy-Item "tenants_module\src\OneLine.API\Controllers\TenantController.cs" `
  "src\OneLine.API\Controllers\"

# ── 4. Mettre à jour Program.cs ──────────────────────────────
Write-Host "`n[4/6] Mise à jour Program.cs..." -ForegroundColor Yellow

$programContent = @'
using OneLine.Auth.Application;
using OneLine.Auth.Infrastructure;
using OneLine.Tenants.Application;
using OneLine.Tenants.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Modules ──────────────────────────────────────────────────
builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddTenantsApplication();
builder.Services.AddTenantsInfrastructure(builder.Configuration);

// ── API ──────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseTenantsMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
'@

Set-Content -Path "src\OneLine.API\Program.cs" -Value $programContent -Encoding UTF8

# ── 5. Build ─────────────────────────────────────────────────
Write-Host "`n[5/6] Build..." -ForegroundColor Yellow
dotnet build

# ── 6. Migration ─────────────────────────────────────────────
Write-Host "`n[6/6] Migration..." -ForegroundColor Yellow

dotnet ef migrations add InitialTenants `
  --project src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj `
  --startup-project tools\OneLine.Migrations\OneLine.Migrations.csproj `
  --context TenantsDbContext `
  --output-dir Persistence\Migrations

dotnet ef database update `
  --project src\Modules\Tenants\OneLine.Tenants.Infrastructure\OneLine.Tenants.Infrastructure.csproj `
  --startup-project tools\OneLine.Migrations\OneLine.Migrations.csproj `
  --context TenantsDbContext

Write-Host "`n=== Module Tenants installé avec succès ! ===" -ForegroundColor Green
Write-Host "Lance l'API : cd src\OneLine.API && dotnet run" -ForegroundColor Cyan
