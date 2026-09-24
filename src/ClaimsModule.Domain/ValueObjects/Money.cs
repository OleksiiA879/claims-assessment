namespace ClaimsModule.Domain.ValueObjects;

public readonly record struct Money
{
    public Money(decimal amount)
    {
        if (decimal.Round(amount, 4) != amount)
            throw new ArgumentOutOfRangeException(nameof(amount), "Money supports at most four decimal places.");

        Amount = amount;
    }

    public decimal Amount { get; }

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);
    public static implicit operator decimal(Money value) => value.Amount;
}
