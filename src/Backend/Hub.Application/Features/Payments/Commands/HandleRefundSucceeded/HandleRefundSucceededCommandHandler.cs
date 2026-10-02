using Ardalis.Result;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Pipelines;
using Microsoft.Extensions.Logging;

namespace Hub.Application.Features.Payments.Commands.HandleRefundSucceeded;

sealed class HandleRefundSucceededCommandHandler(
    ILogger<HandleRefundSucceededCommandHandler> logger
) : IRequestHandler<HandleRefundSucceededCommand, IdResponse>
{
    public Task<Result<IdResponse>> Handle(
        HandleRefundSucceededCommand command,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success(new IdResponse(command.RefundId)));
    }
}
