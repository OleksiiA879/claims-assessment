using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.CreateClaim;

public class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(x => x.LossDate)
            .Must(d => d <= DateTimeOffset.UtcNow)
            .WithMessage("Loss date cannot be in the future.");

        RuleFor(x => x.LossDescription)
            .NotEmpty().WithMessage("Loss description is required.")
            .MinimumLength(20).WithMessage("Loss description is required and must be at least 20 characters.");

        RuleFor(x => x.CauseOfLossCode)
            .NotEmpty().WithMessage("Cause of loss code is required.");

        RuleFor(x => x.Parties)
            .Must(p => p.Any(x => x.PartyRole == Domain.Enums.PartyRole.Claimant))
            .WithMessage("At least one Claimant party is required.");
    }
}
