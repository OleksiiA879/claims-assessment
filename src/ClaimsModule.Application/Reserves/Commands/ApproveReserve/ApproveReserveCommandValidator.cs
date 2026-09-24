using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.ApproveReserve;

public sealed class ApproveReserveCommandValidator : AbstractValidator<ApproveReserveCommand>
{
    public ApproveReserveCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.ReserveHistoryId).NotEmpty();
    }
}
