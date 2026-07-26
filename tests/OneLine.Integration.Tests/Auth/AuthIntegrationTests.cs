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
