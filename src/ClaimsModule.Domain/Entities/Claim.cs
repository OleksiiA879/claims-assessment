using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Events;

namespace ClaimsModule.Domain.Entities;

public class Claim : AggregateRoot
{
    public string ClaimNumber { get; set; } = string.Empty;
    public Guid? PolicyId { get; set; }
    public string? PolicyNumber { get; set; }
    public string? ClientName { get; set; }
    public ClaimStatus Status { get; set; } = ClaimStatus.Draft;
    public string? Severity { get; set; }
    public DateTimeOffset ReportedDate { get; set; }
    public Guid? AssignedHandlerId { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public string? ClosureReason { get; set; }
    public string? Notes { get; set; }
    public bool ManagerOverrideForReserves { get; set; }

    public LossEvent? LossEvent { get; set; }
    public Policy? Policy { get; set; }
    public ICollection<ClaimParty> Parties { get; set; } = [];
    public ICollection<ClaimRiskObject> RiskObjects { get; set; } = [];
    public ICollection<ClaimReserveComponent> ReserveComponents { get; set; } = [];
    public ICollection<ClaimDocument> Documents { get; set; } = [];
    public ICollection<ClaimAuditLog> AuditLogs { get; set; } = [];
    public ICollection<ClaimValidationIssue> ValidationIssues { get; set; } = [];

    public static Claim Create(
        Guid organisationId,
        string claimNumber,
        Guid? policyId,
        string? policyNumber,
        string? clientName,
        Guid userId)
    {
        var claim = new Claim
        {
            Id = Guid.NewGuid(),
            OrganisationId = organisationId,
            ClaimNumber = claimNumber,
            PolicyId = policyId,
            PolicyNumber = policyNumber,
            ClientName = clientName,
            Status = ClaimStatus.Draft,
            ReportedDate = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UserCreated = userId
        };
        claim.RaiseDomainEvent(new ClaimCreatedEvent(claim.Id, claim.ClaimNumber, organisationId, userId));
        return claim;
    }

    public bool HasClaimant() =>
        Parties.Any(p => p.IsActive && p.PartyRole == PartyRole.Claimant);

    public bool HasCriticalValidationIssues() =>
        ValidationIssues.Any(v => v.IsActive && v.Severity == ValidationSeverity.Critical);

    public decimal GetApprovedReserveTotal() =>
        ReserveComponents
            .SelectMany(c => c.History)
            .Where(h => h.ApprovalStatus is ReserveApprovalStatus.Approved or ReserveApprovalStatus.AutoApproved)
            .Sum(h => h.Amount);
}
