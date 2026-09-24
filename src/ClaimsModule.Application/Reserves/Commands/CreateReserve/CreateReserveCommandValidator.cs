using ClaimsModule.Domain.Enums;
using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.CreateReserve;

public class CreateReserveCommandValidator : AbstractValidator<CreateReserveCommand>
{
    public CreateReserveCommandValidator()
    {
        RuleFor(x => x.Amount)
            .Must((cmd, amount) => cmd.TransactionType == ReserveTransactionType.Reverse ? amount < 0 : amount > 0)
            .WithMessage("Reserve additions must be positive and reversals must be negative.");

        RuleFor(x => x.ChangeReason).NotEmpty().MaximumLength(500);
    }
}
