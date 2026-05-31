using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class ClaimStatusTransition
{
    public Guid Id { get; set; }
    public ClaimStatus FromStatus { get; set; }
    public ClaimStatus ToStatus { get; set; }
    public string? RequiredPermission { get; set; }
}
