using MediatR;
using Microsoft.EntityFrameworkCore;
using StripePayment.Application.Common;
using StripePayment.Application.DTOs;
using StripePayment.Application.Interfaces;
using StripePayment.Domain.Entities;

namespace StripePayment.Application.Commands.Payments;

public sealed class CreateCheckoutSessionCommandHandler : IRequestHandler<CreateCheckoutSessionCommand, Result<CheckoutSessionResponse>>
{
    private readonly IPaymentService _paymentService;
    private readonly IApplicationDbContext _context;

    public CreateCheckoutSessionCommandHandler(
        IPaymentService paymentService,
        IApplicationDbContext context)
    {
        _paymentService = paymentService;
        _context = context;
    }

    public async Task<Result<CheckoutSessionResponse>> Handle(
        CreateCheckoutSessionCommand request,
        CancellationToken cancellationToken)
    {
        // Validate customer exists
        var customerGuid = Guid.Parse(request.CustomerId);
        var customerId = new CustomerId(customerGuid);
        
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
        
        if (customer is null)
        {
            return Result.Failure<CheckoutSessionResponse>(
                new Error("Customer.NotFound", "Customer not found"));
        }

        // Validate product exists
        var productGuid = Guid.Parse(request.ProductId);
        var productId = new ProductId(productGuid);
        
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive, cancellationToken);
        
        if (product is null)
        {
            return Result.Failure<CheckoutSessionResponse>(
                new Error("Product.NotFound", "Product not found or inactive"));
        }

        // Validate product type matches request type
        if (request.IsRecurring && !product.IsRecurring)
        {
            return Result.Failure<CheckoutSessionResponse>(
                new Error("Product.InvalidType", "Product is not configured for recurring payments"));
        }

        if (!request.IsRecurring && product.IsRecurring)
        {
            return Result.Failure<CheckoutSessionResponse>(
                new Error("Product.InvalidType", "Product is configured for recurring payments only"));
        }

        // Create checkout session
        var paymentRequest = new PaymentMethodRequest(
            request.CustomerId,
            request.ProductId,
            request.SuccessUrl,
            request.CancelUrl);

        var sessionResult = request.IsRecurring
            ? await _paymentService.CreateSubscriptionSessionAsync(paymentRequest, cancellationToken)
            : await _paymentService.CreateOneTimePaymentSessionAsync(paymentRequest, cancellationToken);

        if (sessionResult.IsFailure)
        {
            return sessionResult;
        }

        // Create order or subscription record
        if (request.IsRecurring)
        {
            var subscription = new Subscription
            {
                Id = SubscriptionId.New(),
                CustomerId = customerId,
                ProductId = productId
            };
            subscription.SetStripeCheckoutSessionId(sessionResult.Value.SessionId);

            _context.Subscriptions.Add(subscription);
            customer.AddSubscription(subscription);
        }
        else
        {
            var order = new Order
            {
                Id = OrderId.New(),
                CustomerId = customerId,
                ProductId = productId,
                Amount = product.Price,
                Currency = product.Currency
            };
            order.SetStripeCheckoutSessionId(sessionResult.Value.SessionId);

            _context.Orders.Add(order);
            customer.AddOrder(order);
        }

        await _context.SaveChangesAsync(cancellationToken);
        
        return sessionResult;
    }
}