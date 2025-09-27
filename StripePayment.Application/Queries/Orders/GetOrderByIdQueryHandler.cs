using MediatR;
using Microsoft.EntityFrameworkCore;
using StripePayment.Application.Common;
using StripePayment.Application.Interfaces;
using StripePayment.Domain.Entities;

namespace StripePayment.Application.Queries.Orders;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<Order>>
{
    private readonly IApplicationDbContext _context;

    public GetOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Order>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.OrderId, out var orderGuid))
        {
            return Result.Failure<Order>(
                new Error("Order.InvalidId", "Invalid order ID format"));
        }

        var orderId = new OrderId(orderGuid);
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<Order>(
                new Error("Order.NotFound", "Order not found"));
        }

        return Result.Success(order);
    }
}