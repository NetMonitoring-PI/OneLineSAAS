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
