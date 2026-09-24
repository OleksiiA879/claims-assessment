using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class ClaimStatusTransition : AuditableEntity
{
    public ClaimStatus FromStatus { get; set; }
    public ClaimStatus ToStatus { get; set; }
    public string? RequiredPermission { get; set; }
}
