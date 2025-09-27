using MediatR;
using Microsoft.EntityFrameworkCore;
using StripePayment.Application.Common;
using StripePayment.Application.DTOs;
using StripePayment.Application.Interfaces;

namespace StripePayment.Application.Commands.Payments;

public sealed class IssueRefundCommandHandler : IRequestHandler<IssueRefundCommand, Result<RefundResponse>>
{
    private readonly IPaymentService _paymentService;
    private readonly IApplicationDbContext _context;

    public IssueRefundCommandHandler(
        IPaymentService paymentService,
        IApplicationDbContext context)
    {
        _paymentService = paymentService;
        _context = context;
    }

    public async Task<Result<RefundResponse>> Handle(
        IssueRefundCommand request,
        CancellationToken cancellationToken)
    {
        // Find the order by charge ID
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.StripeChargeId == request.ChargeId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<RefundResponse>(
                new Error("Order.NotFound", "Order not found for the specified charge ID"));
        }

        // Issue refund through payment service
        var refundRequest = new RefundRequest(request.ChargeId, request.Amount, request.Reason);
        var refundResult = await _paymentService.IssueRefundAsync(refundRequest, cancellationToken);

        if (refundResult.IsFailure)
        {
            return refundResult;
        }

        // Update order status
        order.MarkAsRefunded();
        await _context.SaveChangesAsync(cancellationToken);

        return refundResult;
    }
}