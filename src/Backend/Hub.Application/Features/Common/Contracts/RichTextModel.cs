using Hub.Domain.ValueObjects;

namespace Hub.Application.Features.Common.Contracts;

public sealed record RichTextModel(
    string Text,
    TextFormat Format
);