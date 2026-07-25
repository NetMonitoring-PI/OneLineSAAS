using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Billing.Application.DTOs;
using OneLine.Billing.Application.UseCases.CancelSubscription;
using OneLine.Billing.Application.UseCases.CreateSubscription;
using OneLine.Billing.Application.UseCases.GetSubscription;
using OneLine.Billing.Infrastructure.Webhooks;
using OneLine.Shared.Domain.Result;

namespace OneLine.API.Controllers;

[ApiController]
[Route("api/billing")]
[Produces("application/json")]
public sealed class BillingController : ControllerBase
{
    private readonly ISender _sender;

    public BillingController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("subscribe")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Subscribe(
        [FromBody] CreateSubscriptionCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetSubscription),
                new { tenantId = result.Value!.TenantId }, result.Value)
            : HandleError(result.Error);
    }

    [HttpGet("{tenantId:guid}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscription(Guid tenantId, CancellationToken ct)
    {
        var result = await _sender.Send(
            new GetSubscriptionByTenantQuery(tenantId), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
    }

    [HttpDelete("{tenantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid tenantId, CancellationToken ct)
    {
        var result = await _sender.Send(
            new CancelSubscriptionCommand(tenantId), ct);
        return result.IsSuccess ? NoContent() : HandleError(result.Error);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromServices] StripeWebhookHandler webhookHandler)
    {
        await webhookHandler.HandleAsync(HttpContext);
        return new EmptyResult();
    }

    private IActionResult HandleError(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound   => NotFound(new { error.Code, error.Message }),
            ErrorType.Conflict   => Conflict(new { error.Code, error.Message }),
            ErrorType.Forbidden  => StatusCode(402, new { error.Code, error.Message }),
            ErrorType.Validation => BadRequest(new { error.Code, error.Message }),
            _                    => StatusCode(500, new { error.Code, error.Message })
        };
}
