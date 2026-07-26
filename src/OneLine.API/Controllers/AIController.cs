using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneLine.AI.Application.DTOs;
using OneLine.AI.Application.UseCases.Chat;
using OneLine.AI.Application.UseCases.GetUsage;
using OneLine.Shared.Domain.Result;

namespace OneLine.API.Controllers;

[ApiController]
[Route("api/ai")]
[Produces("application/json")]
public sealed class AIController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<AIController> _logger;

    public AIController(ISender sender, ILogger<AIController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] ChatCommand command, CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur dans Chat: {Message}", ex.Message);
            return StatusCode(500, new
            {
                error = ex.GetType().Name,
                message = ex.Message,
                inner = ex.InnerException?.Message
            });
        }
    }

    [HttpGet("usage/{tenantId:guid}")]
    public async Task<IActionResult> GetUsage(
        Guid tenantId, CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(new GetAIUsageQuery(tenantId), ct);
            return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.GetType().Name,
                message = ex.Message,
                inner = ex.InnerException?.Message
            });
        }
    }

    private IActionResult HandleError(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound   => NotFound(new { error.Code, error.Message }),
            ErrorType.Forbidden  => StatusCode(429, new { error.Code, error.Message }),
            ErrorType.Validation => BadRequest(new { error.Code, error.Message }),
            _                    => StatusCode(500, new { error.Code, error.Message })
        };
}
