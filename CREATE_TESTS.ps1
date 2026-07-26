# ============================================================
# Script Tests Unitaires - One Line SaaS Kit
# Executer depuis : C:\Users\DELL\Projects\OneLine.SaasKit
# ============================================================

Write-Host "=== Tests Unitaires ===" -ForegroundColor Cyan

# ── ETAPE 1 : Packages de test ───────────────────────────────
Write-Host "`n[1/4] Installation packages de test..." -ForegroundColor Yellow

dotnet add tests\OneLine.Shared.Tests\OneLine.Shared.Tests.csproj `
  reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj

dotnet add tests\OneLine.Auth.Tests\OneLine.Auth.Tests.csproj `
  reference src\Modules\Auth\OneLine.Auth.Domain\OneLine.Auth.Domain.csproj

dotnet add tests\OneLine.Auth.Tests\OneLine.Auth.Tests.csproj `
  reference src\Modules\Auth\OneLine.Auth.Application\OneLine.Auth.Application.csproj

dotnet add tests\OneLine.Tenants.Tests\OneLine.Tenants.Tests.csproj `
  reference src\Modules\Tenants\OneLine.Tenants.Domain\OneLine.Tenants.Domain.csproj

dotnet add tests\OneLine.Tenants.Tests\OneLine.Tenants.Tests.csproj `
  reference src\Modules\Tenants\OneLine.Tenants.Application\OneLine.Tenants.Application.csproj

# Packages communs pour tous les projets de test
$testProjects = @(
    "tests\OneLine.Shared.Tests\OneLine.Shared.Tests.csproj",
    "tests\OneLine.Auth.Tests\OneLine.Auth.Tests.csproj",
    "tests\OneLine.Tenants.Tests\OneLine.Tenants.Tests.csproj",
    "tests\OneLine.Integration.Tests\OneLine.Integration.Tests.csproj"
)

foreach ($proj in $testProjects) {
    dotnet add $proj package FluentAssertions --version 6.12.0
    dotnet add $proj package Moq --version 4.20.70
    dotnet add $proj package Microsoft.Extensions.DependencyInjection --version 9.0.0
}

dotnet add tests\OneLine.Integration.Tests\OneLine.Integration.Tests.csproj `
  reference src\OneLine.API\OneLine.API.csproj

dotnet add tests\OneLine.Integration.Tests\OneLine.Integration.Tests.csproj `
  package Testcontainers.PostgreSql --version 3.10.0

dotnet add tests\OneLine.Integration.Tests\OneLine.Integration.Tests.csproj `
  package Microsoft.AspNetCore.Mvc.Testing --version 9.0.0

Write-Host "Packages installes" -ForegroundColor Green

# ── ETAPE 2 : Dossiers ───────────────────────────────────────
Write-Host "`n[2/4] Creation des dossiers..." -ForegroundColor Yellow

$dirs = @(
    "tests\OneLine.Shared.Tests\Result",
    "tests\OneLine.Auth.Tests\Domain",
    "tests\OneLine.Auth.Tests\Application",
    "tests\OneLine.Tenants.Tests\Domain",
    "tests\OneLine.Tenants.Tests\Application",
    "tests\OneLine.Integration.Tests\Auth",
    "tests\OneLine.Integration.Tests\Tenants"
)
foreach ($dir in $dirs) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
Write-Host "Dossiers crees" -ForegroundColor Green

# ── ETAPE 3 : Fichiers de test ───────────────────────────────
Write-Host "`n[3/4] Creation des tests..." -ForegroundColor Yellow

# ── SHARED TESTS ─────────────────────────────────────────────

Set-Content -Path "tests\OneLine.Shared.Tests\Result\ResultTests.cs" -Encoding UTF8 -Value @'
using FluentAssertions;
using OneLine.Shared.Domain.Result;

namespace OneLine.Shared.Tests.Result;

/// <summary>
/// Tests du pattern Result<T>.
/// Verifie que le pattern fonctionne correctement
/// pour les succes et les echecs.
/// </summary>
public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldBeSuccessful()
    {
        // Arrange + Act
        var result = Result<string>.Success("Hello");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be("Hello");
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldBeFailure()
    {
        // Arrange
        var error = Error.NotFound("Test.NotFound", "Resource not found");

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldBeSuccess()
    {
        // Act
        Result<int> result = 42;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldBeFailure()
    {
        // Arrange
        var error = Error.Validation("Test.Invalid", "Invalid data");

        // Act
        Result<int> result = error;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Test.Invalid");
        result.Error.Message.Should().Be("Invalid data");
    }

    [Fact]
    public void ErrorNone_ShouldHaveEmptyCodeAndMessage()
    {
        // Assert
        Error.None.Code.Should().BeEmpty();
        Error.None.Message.Should().BeEmpty();
        Error.None.Type.Should().Be(ErrorType.None);
    }

    [Fact]
    public void ErrorNotFound_ShouldHaveCorrectType()
    {
        var error = Error.NotFound("X.NotFound", "Not found");
        error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void ErrorValidation_ShouldHaveCorrectType()
    {
        var error = Error.Validation("X.Invalid", "Invalid");
        error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void ErrorConflict_ShouldHaveCorrectType()
    {
        var error = Error.Conflict("X.Conflict", "Conflict");
        error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void Result_Void_Success_ShouldWork()
    {
        var result = Result.Success();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Result_Void_Failure_ShouldWork()
    {
        var error = Error.Failure("X.Failed", "Failed");
        var result = Result.Failure(error);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
'@

# ── AUTH DOMAIN TESTS ─────────────────────────────────────────

Set-Content -Path "tests\OneLine.Auth.Tests\Domain\AppUserTests.cs" -Encoding UTF8 -Value @'
using FluentAssertions;
using OneLine.Auth.Domain.Entities;
using OneLine.Auth.Domain.Enums;

namespace OneLine.Auth.Tests.Domain;

/// <summary>
/// Tests de l entite AppUser.
/// Verifie que les regles metier sont correctement implementees.
/// Pattern teste : Factory Method + Rich Domain Model
/// </summary>
public sealed class AppUserTests
{
    private readonly Guid _tenantId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var user = AppUser.Create(
            "Imane", "Test",
            "imane@test.com",
            _tenantId);

        // Assert
        user.FirstName.Should().Be("Imane");
        user.LastName.Should().Be("Test");
        user.Email.Should().Be("imane@test.com");
        user.TenantId.Should().Be(_tenantId);
        user.Role.Should().Be(UserRole.User);
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        // Act
        var user = AppUser.Create("Imane", "Test", "test@test.com", _tenantId);

        // Assert
        user.DomainEvents.Should().HaveCount(1);
        user.DomainEvents[0].Should().BeOfType<OneLine.Auth.Domain.Events.UserCreatedEvent>();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        // Arrange
        var user = AppUser.Create("Imane", "Test", "test@test.com", _tenantId);

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_AfterDeactivate_ShouldSetIsActiveTrue()
    {
        // Arrange
        var user = AppUser.Create("Imane", "Test", "test@test.com", _tenantId);
        user.Deactivate();

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void AssignRole_ShouldChangeRole()
    {
        // Arrange
        var user = AppUser.Create("Imane", "Test", "test@test.com", _tenantId);

        // Act
        user.AssignRole(UserRole.TenantAdmin);

        // Assert
        user.Role.Should().Be(UserRole.TenantAdmin);
    }

    [Fact]
    public void FullName_ShouldCombineFirstAndLastName()
    {
        // Arrange
        var user = AppUser.Create("Imane", "Benali", "test@test.com", _tenantId);

        // Assert
        user.FullName.Should().Be("Imane Benali");
    }

    [Fact]
    public void Create_WithEmptyFirstName_ShouldThrow()
    {
        // Act
        var act = () => AppUser.Create("", "Test", "test@test.com", _tenantId);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullEmail_ShouldThrow()
    {
        // Act
        var act = () => AppUser.Create("Imane", "Test", null!, _tenantId);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var user = AppUser.Create("Imane", "Test", "test@test.com", _tenantId);

        // Act
        user.ClearDomainEvents();

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }
}
'@

Set-Content -Path "tests\OneLine.Auth.Tests\Domain\RefreshTokenTests.cs" -Encoding UTF8 -Value @'
using FluentAssertions;
using OneLine.Auth.Domain.Entities;

namespace OneLine.Auth.Tests.Domain;

/// <summary>
/// Tests de l entite RefreshToken.
/// Verifie le cycle de vie : creation, expiration, revocation.
/// </summary>
public sealed class RefreshTokenTests
{
    [Fact]
    public void Create_ShouldGenerateSecureToken()
    {
        // Act
        var token = RefreshToken.Create(Guid.NewGuid());

        // Assert
        token.Token.Should().NotBeNullOrEmpty();
        token.Token.Length.Should().BeGreaterThan(20);
        token.IsActive.Should().BeTrue();
        token.IsExpired.Should().BeFalse();
        token.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void Create_TwoTokens_ShouldBeDifferent()
    {
        // Act
        var token1 = RefreshToken.Create(Guid.NewGuid());
        var token2 = RefreshToken.Create(Guid.NewGuid());

        // Assert
        token1.Token.Should().NotBe(token2.Token);
    }

    [Fact]
    public void Revoke_ShouldSetIsRevokedTrue()
    {
        // Arrange
        var token = RefreshToken.Create(Guid.NewGuid());

        // Act
        token.Revoke("Test revocation");

        // Assert
        token.IsRevoked.Should().BeTrue();
        token.IsActive.Should().BeFalse();
        token.RevokedAt.Should().NotBeNull();
        token.RevokedReason.Should().Be("Test revocation");
    }

    [Fact]
    public void Revoke_WithReplacedToken_ShouldStoreReplacement()
    {
        // Arrange
        var token = RefreshToken.Create(Guid.NewGuid());
        var newToken = "new-token-value";

        // Act
        token.Revoke("Rotation", newToken);

        // Assert
        token.ReplacedByToken.Should().Be(newToken);
    }

    [Fact]
    public void Create_WithCustomExpiry_ShouldSetCorrectExpiry()
    {
        // Act
        var token = RefreshToken.Create(Guid.NewGuid(), expiryDays: 30);

        // Assert
        token.ExpiresAt.Should()
            .BeCloseTo(DateTime.UtcNow.AddDays(30), TimeSpan.FromMinutes(1));
    }
}
'@

# ── AUTH APPLICATION TESTS ────────────────────────────────────

Set-Content -Path "tests\OneLine.Auth.Tests\Application\RegisterCommandValidatorTests.cs" -Encoding UTF8 -Value @'
using FluentAssertions;
using OneLine.Auth.Application.UseCases.Register;
using OneLine.Auth.Domain.Enums;

namespace OneLine.Auth.Tests.Application;

/// <summary>
/// Tests du validateur RegisterCommand.
/// Verifie que les regles de validation sont correctes.
/// Pattern teste : FluentValidation
/// </summary>
public sealed class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    [Fact]
    public void Validate_WithValidData_ShouldBeValid()
    {
        // Arrange
        var command = new RegisterCommand(
            "Imane", "Test",
            "imane@test.com",
            "Test1234!",
            "Test1234!",
            _tenantId);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyFirstName_ShouldBeInvalid()
    {
        var command = new RegisterCommand(
            "", "Test", "test@test.com",
            "Test1234!", "Test1234!", _tenantId);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(RegisterCommand.FirstName));
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldBeInvalid()
    {
        var command = new RegisterCommand(
            "Imane", "Test", "not-an-email",
            "Test1234!", "Test1234!", _tenantId);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(RegisterCommand.Email));
    }

    [Fact]
    public void Validate_WithShortPassword_ShouldBeInvalid()
    {
        var command = new RegisterCommand(
            "Imane", "Test", "test@test.com",
            "short", "short", _tenantId);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(RegisterCommand.Password));
    }

    [Fact]
    public void Validate_WithPasswordWithoutUppercase_ShouldBeInvalid()
    {
        var command = new RegisterCommand(
            "Imane", "Test", "test@test.com",
            "test1234!", "test1234!", _tenantId);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithPasswordWithoutDigit_ShouldBeInvalid()
    {
        var command = new RegisterCommand(
            "Imane", "Test", "test@test.com",
            "TestPass!", "TestPass!", _tenantId);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithMismatchedPasswords_ShouldBeInvalid()
    {
        var command = new RegisterCommand(
            "Imane", "Test", "test@test.com",
            "Test1234!", "Different1!", _tenantId);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(RegisterCommand.ConfirmPassword));
    }

    [Fact]
    public void Validate_WithEmptyTenantId_ShouldBeInvalid()
    {
        var command = new RegisterCommand(
            "Imane", "Test", "test@test.com",
            "Test1234!", "Test1234!", Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(RegisterCommand.TenantId));
    }
}
'@

# ── TENANT DOMAIN TESTS ───────────────────────────────────────

Set-Content -Path "tests\OneLine.Tenants.Tests\Domain\TenantTests.cs" -Encoding UTF8 -Value @'
using FluentAssertions;
using OneLine.Tenants.Domain.Entities;
using OneLine.Tenants.Domain.Enums;

namespace OneLine.Tenants.Tests.Domain;

/// <summary>
/// Tests de l entite Tenant.
/// Verifie les regles metier du domaine Tenants.
/// </summary>
public sealed class TenantTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var tenant = Tenant.Create(
            "Mon Startup",
            "monstartup",
            "contact@monstartup.com");

        // Assert
        tenant.Name.Should().Be("Mon Startup");
        tenant.Subdomain.Should().Be("monstartup");
        tenant.ContactEmail.Should().Be("contact@monstartup.com");
        tenant.Plan.Should().Be(TenantPlan.Free);
        tenant.Status.Should().Be(TenantStatus.Trial);
        tenant.IsActive.Should().BeTrue();
        tenant.Id.Should().NotBeEmpty();
        tenant.TrialEndsAt.Should().NotBeNull();
    }

    [Fact]
    public void Create_SubdomainShouldBeLowercase()
    {
        // Act
        var tenant = Tenant.Create("Test", "UPPERCASE");

        // Assert
        tenant.Subdomain.Should().Be("uppercase");
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        // Act
        var tenant = Tenant.Create("Test", "test");

        // Assert
        tenant.DomainEvents.Should().HaveCount(1);
        tenant.DomainEvents[0].Should()
            .BeOfType<OneLine.Tenants.Domain.Events.TenantCreatedEvent>();
    }

    [Fact]
    public void Create_TrialShouldEndIn14Days()
    {
        // Act
        var tenant = Tenant.Create("Test", "test");

        // Assert
        tenant.TrialEndsAt.Should()
            .BeCloseTo(DateTime.UtcNow.AddDays(14), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Activate_ShouldSetStatusToActive()
    {
        // Arrange
        var tenant = Tenant.Create("Test", "test");

        // Act
        tenant.Activate();

        // Assert
        tenant.Status.Should().Be(TenantStatus.Active);
        tenant.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Suspend_ShouldSetStatusToSuspended()
    {
        // Arrange
        var tenant = Tenant.Create("Test", "test");
        tenant.Activate();

        // Act
        tenant.Suspend();

        // Assert
        tenant.Status.Should().Be(TenantStatus.Suspended);
        tenant.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Upgrade_ShouldChangePlan()
    {
        // Arrange
        var tenant = Tenant.Create("Test", "test");

        // Act
        tenant.Upgrade(TenantPlan.Pro);

        // Assert
        tenant.Plan.Should().Be(TenantPlan.Pro);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrow()
    {
        // Act
        var act = () => Tenant.Create("", "test");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptySubdomain_ShouldThrow()
    {
        // Act
        var act = () => Tenant.Create("Test", "");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateInfo_ShouldChangeName()
    {
        // Arrange
        var tenant = Tenant.Create("Old Name", "test");

        // Act
        tenant.UpdateInfo("New Name", "New description");

        // Assert
        tenant.Name.Should().Be("New Name");
        tenant.Description.Should().Be("New description");
    }
}
'@

Set-Content -Path "tests\OneLine.Tenants.Tests\Application\CreateTenantCommandValidatorTests.cs" -Encoding UTF8 -Value @'
using FluentAssertions;
using OneLine.Tenants.Application.UseCases.CreateTenant;

namespace OneLine.Tenants.Tests.Application;

/// <summary>
/// Tests du validateur CreateTenantCommand.
/// </summary>
public sealed class CreateTenantCommandValidatorTests
{
    private readonly CreateTenantCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldBeValid()
    {
        var command = new CreateTenantCommand(
            "Mon Startup", "monstartup", "contact@test.com");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldBeInvalid()
    {
        var command = new CreateTenantCommand("", "test");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateTenantCommand.Name));
    }

    [Fact]
    public void Validate_WithUppercaseSubdomain_ShouldBeInvalid()
    {
        var command = new CreateTenantCommand("Test", "UPPERCASE");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateTenantCommand.Subdomain));
    }

    [Fact]
    public void Validate_WithSpecialCharsInSubdomain_ShouldBeInvalid()
    {
        var command = new CreateTenantCommand("Test", "mon startup!");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldBeInvalid()
    {
        var command = new CreateTenantCommand(
            "Test", "test", "not-an-email");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateTenantCommand.ContactEmail));
    }

    [Fact]
    public void Validate_WithNullEmail_ShouldBeValid()
    {
        // L email est optionnel
        var command = new CreateTenantCommand("Test", "test", null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithTooLongSubdomain_ShouldBeInvalid()
    {
        var longSubdomain = new string('a', 51);
        var command = new CreateTenantCommand("Test", longSubdomain);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
'@

# ── INTEGRATION TESTS ─────────────────────────────────────────

Set-Content -Path "tests\OneLine.Integration.Tests\Auth\AuthIntegrationTests.cs" -Encoding UTF8 -Value @'
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OneLine.Integration.Tests.Auth;

/// <summary>
/// Tests d integration pour le module Auth.
/// Utilise WebApplicationFactory pour tester l API reelle.
///
/// Note : Ces tests necessitent une DB PostgreSQL.
/// Pour les lancer avec Testcontainers, decommentez la fixture.
/// </summary>
public sealed class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturn201()
    {
        // Arrange
        var request = new
        {
            firstName = "Integration",
            lastName = "Test",
            email = $"integration_{Guid.NewGuid()}@test.com",
            password = "Test1234!",
            confirmPassword = "Test1234!",
            tenantId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<JsonDocument>();
        body.Should().NotBeNull();

        var root = body!.RootElement;
        root.TryGetProperty("accessToken", out var accessToken).Should().BeTrue();
        accessToken.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturn400()
    {
        // Arrange
        var request = new
        {
            firstName = "Test",
            lastName = "User",
            email = "not-an-email",
            password = "Test1234!",
            confirmPassword = "Test1234!",
            tenantId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200()
    {
        // Arrange - creer un compte d abord
        var email = $"login_test_{Guid.NewGuid()}@test.com";
        var password = "Test1234!";

        var registerRequest = new
        {
            firstName = "Login",
            lastName = "Test",
            email = email,
            password = password,
            confirmPassword = password,
            tenantId = Guid.NewGuid()
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Act - se connecter
        var loginRequest = new { email, password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonDocument>();
        var root = body!.RootElement;
        root.TryGetProperty("accessToken", out var token).Should().BeTrue();
        token.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturn401()
    {
        // Arrange
        var loginRequest = new
        {
            email = "wrong@test.com",
            password = "WrongPass1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
'@

Write-Host "Tests crees" -ForegroundColor Green

# ── ETAPE 4 : Build et lancer les tests ──────────────────────
Write-Host "`n[4/4] Build et execution des tests unitaires..." -ForegroundColor Yellow

dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nExecution des tests unitaires (sans integration)..." -ForegroundColor Cyan

    dotnet test tests\OneLine.Shared.Tests\OneLine.Shared.Tests.csproj --no-build -v normal
    dotnet test tests\OneLine.Auth.Tests\OneLine.Auth.Tests.csproj --no-build -v normal
    dotnet test tests\OneLine.Tenants.Tests\OneLine.Tenants.Tests.csproj --no-build -v normal

    Write-Host "`n=== TESTS TERMINES ===" -ForegroundColor Green
    Write-Host "`nPour les tests d integration (necessite la DB) :" -ForegroundColor Cyan
    Write-Host "dotnet test tests\OneLine.Integration.Tests\OneLine.Integration.Tests.csproj" -ForegroundColor Gray
} else {
    Write-Host "`n=== BUILD ECHOUE ===" -ForegroundColor Red
}
