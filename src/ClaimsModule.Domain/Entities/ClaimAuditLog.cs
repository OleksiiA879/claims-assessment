using ClaimsModule.Domain.Common;

namespace ClaimsModule.Domain.Entities;

/// <summary>Append-only audit log — no updates or deletes at application layer.</summary>
public class ClaimAuditLog : AuditableEntity
{
    public Guid ClaimId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? CorrelationId { get; set; }
    public Guid? CreatedByUserId { get; set; }

    public Claim? Claim { get; set; }
}
