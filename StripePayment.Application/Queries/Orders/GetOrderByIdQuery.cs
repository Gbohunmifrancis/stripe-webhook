using MediatR;
using StripePayment.Application.Common;
using StripePayment.Domain.Entities;

namespace StripePayment.Application.Queries.Orders;

public sealed record GetOrderByIdQuery(string OrderId) : IRequest<Result<Order>>;