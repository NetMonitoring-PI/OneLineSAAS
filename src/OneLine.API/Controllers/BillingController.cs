using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly StripeWebhookHandler _webhookHandler;

    public BillingController(ISender sender, StripeWebhookHandler webhookHandler)
    {
        _sender = sender;
        _webhookHandler = webhookHandler;
    }

    /// <summary>CrÃ©er un abonnement pour un tenant</summary>
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

    /// <summary>RÃ©cupÃ©rer l abonnement d un tenant</summary>
    [HttpGet("{tenantId:guid}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscription(Guid tenantId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetSubscriptionByTenantQuery(tenantId), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
    }

    /// <summary>Annuler un abonnement</summary>
    [HttpDelete("{tenantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid tenantId, CancellationToken ct)
    {
        var result = await _sender.Send(new CancelSubscriptionCommand(tenantId), ct);
        return result.IsSuccess ? NoContent() : HandleError(result.Error);
    }

    /// <summary>Webhook Stripe -- ne pas protÃ©ger avec [Authorize]</summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook()
    {
        await _webhookHandler.HandleAsync(HttpContext);
        return new EmptyResult();
    }

    private IActionResult HandleError(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => NotFound(new { error.Code, error.Message }),
            ErrorType.Conflict => Conflict(new { error.Code, error.Message }),
            ErrorType.Forbidden => StatusCode(402, new { error.Code, error.Message }),
            ErrorType.Validation => BadRequest(new { error.Code, error.Message }),
            _ => StatusCode(500, new { error.Code, error.Message })
        };
}
