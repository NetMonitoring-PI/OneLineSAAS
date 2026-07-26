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
