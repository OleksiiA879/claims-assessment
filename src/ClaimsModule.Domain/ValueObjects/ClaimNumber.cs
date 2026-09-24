using System.Text.RegularExpressions;

namespace ClaimsModule.Domain.ValueObjects;

public sealed partial record ClaimNumber
{
    private ClaimNumber(string value) => Value = value;

    public string Value { get; }

    public static ClaimNumber Create(int year, int sequence)
    {
        if (year < 2000 || sequence <= 0)
            throw new ArgumentOutOfRangeException(nameof(sequence), "Claim number values are invalid.");

        return new ClaimNumber($"CLM-{year}-{sequence:D7}");
    }

    public static ClaimNumber Parse(string value)
    {
        if (!ClaimNumberPattern().IsMatch(value))
            throw new ArgumentException("Claim number must use the CLM-YYYY-NNNNNNN format.", nameof(value));

        return new ClaimNumber(value);
    }

    public override string ToString() => Value;

    [GeneratedRegex("^CLM-\\d{4}-\\d{7}$", RegexOptions.CultureInvariant)]
    private static partial Regex ClaimNumberPattern();
}
