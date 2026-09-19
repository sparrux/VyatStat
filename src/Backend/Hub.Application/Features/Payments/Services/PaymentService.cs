using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Abstractions.Payments;
using Hub.Application.Features.Payments.Contracts;
using Hub.Domain.Payments;
using Hub.Domain.Payments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Services;

sealed class PaymentService(
    IPaymentsDbContext paymentsDbContext,
    IPaymentGatewayResolver gatewayResolver
) : IPaymentService
{
    public async Task<Result<Payment>> FindPaymentAsync(
        Guid? referenceId, 
        string idempotencyKey, 
        CancellationToken cancellationToken)
    {
        var payment = await paymentsDbContext.Payments
            .Include(x => x.Attempts)
            .FirstOrDefaultAsync(x =>
                x.IdempotencyKey == idempotencyKey && (referenceId == null 
                                                       || x.ReferenceId == referenceId), cancellationToken);

        if (payment is null)
            return Result.NotFound("Payment not found");
        
        return payment;
    }

    public async Task<Result<CreatePaymentResult>> CreatePaymentAsync(
        CreatePaymentRequest request, 
        CancellationToken cancellationToken)
    {
        var idempotencyKey = NormalizeIdempotency(request.IdempotencyKey);

        var existing = await FindByIdempotencyKey(idempotencyKey, cancellationToken);
        if (existing != null)
        {
            if (existing.CustomerId != request.CustomerId)
                return Result.Conflict("Idempotency key is already used");

            return new CreatePaymentResult(existing);
        }
        
        var amount = Money.Create(request.Amount, request.Currency);
        if (!amount.IsSuccess)
            return amount.Map();

        Customer? customer;

        if (request is { IsAnonymous: false, CustomerId: null })
        {
            return Result.Conflict("Customer id is required because is not anonymous");
        }
        
        if (request is { IsAnonymous: false, CustomerId: not null })
        {
            var customerResult = await GetOrCreateCustomer(request.CustomerId.Value, cancellationToken);
            if (!customerResult.IsSuccess)
                return customerResult.Map();
            
            customer = customerResult.Value;
        }
        else
        {
            customer = null;
        }

        var payment = Payment.Create(
            amount.Value,
            request.Purpose,
            request.ReferenceId,
            customer?.Id,
            idempotencyKey);
        if (!payment.IsSuccess)
            return payment.Map();

        await paymentsDbContext.Payments.AddAsync(payment.Value, cancellationToken);
        await paymentsDbContext.SaveChangesAsync(cancellationToken);

        return new CreatePaymentResult(payment);
    }

    public async Task<Result<PaymentCheckoutResult>> StartCheckoutAsync(
        Payment payment,
        PaymentCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        if (payment.Status is PaymentStatus.Succeeded or PaymentStatus.Cancelled)
            return Result.Success(new PaymentCheckoutResult(payment, ApprovalUrl: null));

        var gateway = gatewayResolver.Resolve(request.Provider);
        if (!gateway.IsSuccess)
            return gateway.Map();

        var attempt = payment.Attempts
            .OrderByDescending(x => x.AttemptNumber)
            .FirstOrDefault();

        if (attempt is null || attempt.Status is PaymentAttemptStatus.Failed or PaymentAttemptStatus.Cancelled)
        {
            var provider = ProviderName.Create(gateway.Value.Name);
            if (!provider.IsSuccess)
                return provider.Map();

            var started = payment.StartAttempt(provider.Value, attempt?.ProviderPaymentId);
            if (!started.IsSuccess)
                return started.Map();

            attempt = started.Value;
            await paymentsDbContext.SaveChangesAsync(cancellationToken);
        }

        var resumed = await CheckoutWithGateway(
            payment,
            attempt,
            gateway.Value,
            request,
            cancellationToken);

        return resumed.IsSuccess
            ? Result.Success(resumed.Value)
            : resumed;
    }

    async Task<Result<PaymentCheckoutResult>> CheckoutWithGateway(
        Payment payment,
        PaymentAttempt attempt,
        IPaymentGateway gateway,
        PaymentCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        var created = await gateway.CreatePaymentAsync(
            new CreateGatewayPaymentRequest(
                payment.Amount,
                request.ReferenceId.ToString(),
                NormalizeDescription(request.Description),
                PaymentCheckout.GatewayIdempotencyKey(attempt, "create"),
                request.ReturnUrl,
                request.CancelUrl),
            cancellationToken);

        if (!created.IsSuccess)
        {
            payment.FailAttempt(attempt.Id, null, created.Errors.FirstOrDefault());
            await paymentsDbContext.SaveChangesAsync(cancellationToken);
            return created.Map();
        }

        var applied = PaymentCheckout.ApplyProviderResult(
            payment,
            attempt.Id,
            created.Value.ProviderPaymentId,
            created.Value.Status);

        if (!applied.IsSuccess)
        {
            await paymentsDbContext.SaveChangesAsync(cancellationToken);
            return applied.Map();
        }

        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        return Result.Created(new PaymentCheckoutResult(payment, created.Value.ApprovalUrl));
    }

    async Task<Result<Customer>> GetOrCreateCustomer(Guid userId, CancellationToken cancellationToken)
    {
        var customer = await paymentsDbContext.Customers
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (customer is not null)
            return Result.Success(customer);

        var created = Customer.Create(userId);
        if (!created.IsSuccess)
            return created;

        await paymentsDbContext.Customers.AddAsync(created.Value, cancellationToken);
        return created;
    }

    async Task<Payment?> FindByIdempotencyKey(
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var payment = await paymentsDbContext.Payments
            .Include(x => x.Attempts)
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

        return payment;
    }

    static string NormalizeIdempotency(string value) => value.Trim();
    
    static string? NormalizeDescription(string? value) => value?.Trim();
}