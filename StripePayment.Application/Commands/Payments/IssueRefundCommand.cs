using MediatR;
using StripePayment.Application.Common;
using StripePayment.Application.DTOs;

namespace StripePayment.Application.Commands.Payments;

public sealed record IssueRefundCommand(
    string ChargeId,
    decimal? Amount = null,
    string? Reason = null
) : IRequest<Result<RefundResponse>>;