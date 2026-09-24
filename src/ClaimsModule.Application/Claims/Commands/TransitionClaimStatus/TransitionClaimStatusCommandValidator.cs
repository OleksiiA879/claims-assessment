using ClaimsModule.Domain.Enums;
using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.TransitionClaimStatus;

public sealed class TransitionClaimStatusCommandValidator : AbstractValidator<TransitionClaimStatusCommand>
{
    public TransitionClaimStatusCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.Reason)
            .NotEmpty()
            .When(x => x.TargetStatus is ClaimStatus.Closed or ClaimStatus.Withdrawn)
            .WithMessage("A reason is required when closing or withdrawing a claim.");
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
