using MediatR;
using StripePayment.Application.Common;

namespace StripePayment.Application.Commands.Webhooks;

public sealed record ProcessWebhookCommand(
    string EventType,
    string Payload,
    string Signature
) : IRequest<Result>;