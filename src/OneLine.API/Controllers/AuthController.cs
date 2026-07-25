using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneLine.Auth.Application.DTOs;
using OneLine.Auth.Application.UseCases.Login;
using OneLine.Auth.Application.UseCases.Register;
using OneLine.Shared.Domain.Result;

namespace OneLine.API.Controllers;

/// <summary>
/// Endpoints d'authentification.
///
/// Pattern : Thin Controller
/// → Le controller ne contient AUCUNE logique métier
/// → Il reçoit la requête, envoie la Command via MediatR
/// → MediatR trouve le bon Handler et retourne le résultat
/// → Le controller traduit Result<T> en HTTP response
///
/// C'est tout ce que fait un bon controller.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;
    // ISender = interface MediatR pour envoyer Commands/Queries
    // On injecte ISender et pas IMediator directement
    // → plus léger, expose uniquement Send()

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Créer un nouveau compte</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Register), result.Value)
            : HandleError(result.Error);
    }

    /// <summary>Se connecter</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
    {
        // Récupérer l'IP pour le refresh token
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var commandWithIp = command with { IpAddress = ip };

        var result = await _sender.Send(commandWithIp, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    // ── Méthode privée : traduit Error → HTTP Response ───────
    // Pattern : centralisé dans le controller
    // Évite de répéter if/else dans chaque action
    private IActionResult HandleError(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => NotFound(
                CreateProblem(error, StatusCodes.Status404NotFound)),

            ErrorType.Unauthorized => Unauthorized(
                CreateProblem(error, StatusCodes.Status401Unauthorized)),

            ErrorType.Forbidden => StatusCode(
                StatusCodes.Status403Forbidden,
                CreateProblem(error, StatusCodes.Status403Forbidden)),

            ErrorType.Conflict => Conflict(
                CreateProblem(error, StatusCodes.Status409Conflict)),

            ErrorType.Validation => BadRequest(
                CreateProblem(error, StatusCodes.Status400BadRequest)),

            _ => StatusCode(
                StatusCodes.Status500InternalServerError,
                CreateProblem(error, StatusCodes.Status500InternalServerError))
        };
    }

    private static object CreateProblem(Error error, int status)
        => new
        {
            type = $"https://httpstatuses.com/{status}",
            title = error.Code,
            detail = error.Message,
            status
        };
}