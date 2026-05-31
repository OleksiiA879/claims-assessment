using ClaimsModule.Domain.Common;

namespace ClaimsModule.Domain.Entities;

public class ClaimRiskObject : AuditableEntity
{
    public Guid ClaimId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string AssetDescription { get; set; } = string.Empty;
    public string? DamageDescription { get; set; }
    public bool IsPrimary { get; set; }
    public string? AssetReference { get; set; }

    public Claim? Claim { get; set; }
}
