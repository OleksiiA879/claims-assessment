using ClaimsModule.Domain.Enums;
using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.AddClaimParty;

public sealed class AddClaimPartyCommandValidator : AbstractValidator<AddClaimPartyCommand>
{
    public AddClaimPartyCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .When(x => x.PartyType == PartyType.Company)
            .WithMessage("Company name is required for a company party.");
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .When(x => x.PartyType == PartyType.Person)
            .WithMessage("First name is required for a person party.");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .When(x => x.PartyType == PartyType.Person)
            .WithMessage("Last name is required for a person party.");
    }
}
