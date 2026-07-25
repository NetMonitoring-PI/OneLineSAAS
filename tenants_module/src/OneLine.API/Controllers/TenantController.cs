using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLine.Shared.Domain.Result;
using OneLine.Tenants.Application.DTOs;
using OneLine.Tenants.Application.UseCases.CreateTenant;
using OneLine.Tenants.Application.UseCases.GetTenant;

namespace OneLine.API.Controllers;

[ApiController]
[Route("api/tenants")]
[Produces("application/json")]
public sealed class TenantController : ControllerBase
{
    private readonly ISender _sender;

    public TenantController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Créer un nouveau tenant</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById),
                new { id = result.Value!.Id }, result.Value)
            : HandleError(result.Error);
    }

    /// <summary>Obtenir un tenant par son Id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetTenantQuery(id), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    private IActionResult HandleError(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => NotFound(new { error.Code, error.Message }),
            ErrorType.Conflict => Conflict(new { error.Code, error.Message }),
            ErrorType.Validation => BadRequest(new { error.Code, error.Message }),
            _ => StatusCode(500, new { error.Code, error.Message })
        };
}
