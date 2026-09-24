using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.RetractReserve;

public sealed class RetractReserveCommandValidator : AbstractValidator<RetractReserveCommand>
{
    public RetractReserveCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.ReserveHistoryId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
