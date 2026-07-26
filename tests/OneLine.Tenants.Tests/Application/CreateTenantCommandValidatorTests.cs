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
