namespace ClaimsModule.Domain.Entities;

public class Policy
{
    public Guid Id { get; set; }
    public Guid OrganisationId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateOnly EffectiveDate { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public string Status { get; set; } = "Active";
    public string CoverageTypes { get; set; } = string.Empty;
}
