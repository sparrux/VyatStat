using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Payments.Commands.HandlePaymentCancelled;
using Hub.Application.Features.Payments.Messages;
using Hub.Application.Pipelines;
using MassTransit;

namespace Hub.Infrastructure.Messaging.Consumers;

public sealed class HandlePaymentCancelledCommandHandler(
    IRequestHandler<HandlePaymentCancelledCommand, IdResponse> handler
) : IConsumer<PaymentCancelled>
{
    public Task Consume(ConsumeContext<PaymentCancelled> context) =>
        RequestHandlerConsume.Consume(
            handler,
            new HandlePaymentCancelledCommand(
                context.Message.PaymentId,
                context.Message.Purpose,
                context.Message.ReferenceId),
            context.CancellationToken);
}