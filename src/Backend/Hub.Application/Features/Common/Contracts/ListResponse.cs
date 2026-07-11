namespace Hub.Application.Features.Common.Contracts;

public sealed record ListResponse<T>(
    IReadOnlyCollection<T> Values,
    int Total
);