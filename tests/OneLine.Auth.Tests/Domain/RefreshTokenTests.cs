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
