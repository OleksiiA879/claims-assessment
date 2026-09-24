using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.SetManagerReserveOverride;

public sealed class SetManagerReserveOverrideCommandValidator : AbstractValidator<SetManagerReserveOverrideCommand>
{
    public SetManagerReserveOverrideCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
