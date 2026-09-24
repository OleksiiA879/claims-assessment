using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.RejectReserve;

public sealed class RejectReserveCommandValidator : AbstractValidator<RejectReserveCommand>
{
    public RejectReserveCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.ReserveHistoryId).NotEmpty();
        RuleFor(x => x.RejectionReason).NotEmpty().MaximumLength(500);
    }
}
