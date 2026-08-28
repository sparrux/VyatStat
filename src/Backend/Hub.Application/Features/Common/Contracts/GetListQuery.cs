namespace Hub.Application.Features.Common.Contracts;

public abstract record GetListQuery(
    int Take,
    int Skip
);