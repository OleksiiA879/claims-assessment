using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Services;

public static class ClaimStatusMachine
{
    private static readonly Dictionary<ClaimStatus, ClaimStatus[]> AllowedTransitions = new()
    {
        [ClaimStatus.Draft] = [ClaimStatus.Open],
        [ClaimStatus.Open] = [ClaimStatus.UnderInvestigation, ClaimStatus.PendingPayment, ClaimStatus.Closed, ClaimStatus.Withdrawn],
        [ClaimStatus.UnderInvestigation] = [ClaimStatus.Open, ClaimStatus.PendingPayment, ClaimStatus.Closed, ClaimStatus.Withdrawn],
        [ClaimStatus.PendingPayment] = [ClaimStatus.Closed],
        [ClaimStatus.Closed] = [ClaimStatus.Reopened],
        [ClaimStatus.Reopened] = [ClaimStatus.Open],
        [ClaimStatus.Withdrawn] = [],
        [ClaimStatus.SlaBreached] = [ClaimStatus.Open, ClaimStatus.UnderInvestigation]
    };

    public static bool IsTransitionAllowed(ClaimStatus from, ClaimStatus to) =>
        AllowedTransitions.TryGetValue(from, out var targets) && targets.Contains(to);

    public static IReadOnlyList<ClaimStatus> GetNextStatuses(ClaimStatus current) =>
        AllowedTransitions.TryGetValue(current, out var targets)
            ? targets
            : Array.Empty<ClaimStatus>();
}
