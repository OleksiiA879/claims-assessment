using ClaimsModule.Domain.Enums;
using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.CreateReserve;

public class CreateReserveCommandValidator : AbstractValidator<CreateReserveCommand>
{
    public CreateReserveCommandValidator()
    {
        RuleFor(x => x.Amount)
            .Must((cmd, amount) => cmd.Component == ReserveComponentType.SubrogationRecoverable || amount > 0)
            .WithMessage("Reserve amount must be greater than zero.");

        RuleFor(x => x.ChangeReason).NotEmpty().MaximumLength(500);
    }
}
