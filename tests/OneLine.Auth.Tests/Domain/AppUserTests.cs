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
