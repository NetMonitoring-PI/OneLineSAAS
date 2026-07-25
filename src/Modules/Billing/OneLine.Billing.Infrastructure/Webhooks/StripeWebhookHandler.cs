using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Billing.Infrastructure.Options;
using Stripe;

namespace OneLine.Billing.Infrastructure.Webhooks;

public sealed class StripeWebhookHandler
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StripeOptions _options;

    public StripeWebhookHandler(
        ISubscriptionRepository subscriptionRepo,
        IInvoiceRepository invoiceRepo,
        IUnitOfWork unitOfWork,
        IOptions<StripeOptions> options)
    {
        _subscriptionRepo = subscriptionRepo;
        _invoiceRepo = invoiceRepo;
        _unitOfWork = unitOfWork;
        _options = options.Value;
    }

    public async Task HandleAsync(HttpContext context)
    {
        using var reader = new StreamReader(context.Request.Body);
        var payload = await reader.ReadToEndAsync();
        var signature = context.Request.Headers["Stripe-Signature"].ToString();

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                payload, signature, _options.WebhookSecret);
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await (stripeEvent.Type switch
        {
            "invoice.payment_succeeded" => HandlePaymentSucceededAsync(stripeEvent),
            "invoice.payment_failed"    => HandlePaymentFailedAsync(stripeEvent),
            "customer.subscription.deleted" => HandleSubscriptionDeletedAsync(stripeEvent),
            _ => Task.CompletedTask
        });

        context.Response.StatusCode = StatusCodes.Status200OK;
    }

    private async Task HandlePaymentSucceededAsync(Event stripeEvent)
    {
        var stripeInvoice = stripeEvent.Data.Object as Stripe.Invoice;
        if (stripeInvoice?.SubscriptionId is null) return;

        var sub = await _subscriptionRepo
            .GetByStripeSubscriptionIdAsync(stripeInvoice.SubscriptionId);
        if (sub is null) return;

        // Créer la facture dans notre DB
        var dbInvoice = Domain.Entities.Invoice.Create(
            sub.TenantId, sub.Id,
            stripeInvoice.AmountPaid / 100m,
            stripeInvoice.Id,
            stripeInvoice.HostedInvoiceUrl);

        dbInvoice.MarkAsPaid();
        await _invoiceRepo.AddAsync(dbInvoice);

        var periodEnd = stripeInvoice.Lines.Data
            .FirstOrDefault()?.Period?.End ?? DateTime.UtcNow.AddMonths(1);

        sub.Activate(periodEnd, stripeInvoice.SubscriptionId);
        _subscriptionRepo.Update(sub);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandlePaymentFailedAsync(Event stripeEvent)
    {
        var stripeInvoice = stripeEvent.Data.Object as Stripe.Invoice;
        if (stripeInvoice?.SubscriptionId is null) return;

        var sub = await _subscriptionRepo
            .GetByStripeSubscriptionIdAsync(stripeInvoice.SubscriptionId);
        if (sub is null) return;

        sub.MarkAsPastDue();
        _subscriptionRepo.Update(sub);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleSubscriptionDeletedAsync(Event stripeEvent)
    {
        var stripeSub = stripeEvent.Data.Object as Stripe.Subscription;
        if (stripeSub is null) return;

        var sub = await _subscriptionRepo
            .GetByStripeSubscriptionIdAsync(stripeSub.Id);
        if (sub is null) return;

        sub.Cancel();
        _subscriptionRepo.Update(sub);
        await _unitOfWork.SaveChangesAsync();
    }
}
