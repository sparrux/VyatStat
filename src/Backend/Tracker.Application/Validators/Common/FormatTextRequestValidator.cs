using FluentValidation;
using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Validators.Common;

public sealed class FormatTextRequestValidator : AbstractValidator<FormatTextRequest>
{
    public FormatTextRequestValidator()
    {
        RuleFor(x => x.Text)
            .Cascade(CascadeMode.Stop)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .WithMessage("Text is required");

        RuleFor(x => x.Format)
            .IsInEnum()
            .WithMessage("Invalid text format");
    }
}
