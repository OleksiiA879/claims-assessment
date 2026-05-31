using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class ClaimReserveComponent : AggregateRoot
{
    public Guid ClaimId { get; set; }
    public ReserveComponentType Component { get; set; }
    public decimal CurrentAmount { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }

    public Claim? Claim { get; set; }
    public ICollection<ReserveHistory> History { get; set; } = [];

    public decimal ComputeCurrentAmount() =>
        History
            .Where(h => h.ApprovalStatus is ReserveApprovalStatus.Approved or ReserveApprovalStatus.AutoApproved)
            .Sum(h => h.Amount);
}
