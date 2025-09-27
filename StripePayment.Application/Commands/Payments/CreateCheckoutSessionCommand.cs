using MediatR;
using StripePayment.Application.Common;
using StripePayment.Application.DTOs;

namespace StripePayment.Application.Commands.Payments;

public sealed record CreateCheckoutSessionCommand(
    string CustomerId,
    string ProductId,
    bool IsRecurring = false,
    string? SuccessUrl = null,
    string? CancelUrl = null
) : IRequest<Result<CheckoutSessionResponse>>;