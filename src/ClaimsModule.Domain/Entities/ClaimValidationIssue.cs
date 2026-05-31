using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class ClaimValidationIssue : AuditableEntity
{
    public Guid ClaimId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAcknowledged { get; set; }

    public Claim? Claim { get; set; }
}
