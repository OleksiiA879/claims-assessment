using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Services;

public static class ReserveAuthorityService
{
    public static AuthorityLevel GetRequiredAuthority(decimal amount) =>
        amount switch
        {
            <= SeedConstants.AutoApprovalThreshold => AuthorityLevel.Auto,
            <= SeedConstants.SupervisorApprovalThreshold => AuthorityLevel.Supervisor,
            _ => AuthorityLevel.Manager
        };

    public static ReserveApprovalStatus DetermineInitialApprovalStatus(decimal amount) =>
        GetRequiredAuthority(amount) == AuthorityLevel.Auto
            ? ReserveApprovalStatus.AutoApproved
            : ReserveApprovalStatus.PendingApproval;

    public static bool CanApprove(string role, decimal amount, AuthorityLevel required)
    {
        if (required == AuthorityLevel.Auto) return true;
        if (required == AuthorityLevel.Supervisor)
            return role is "supervisor" or "manager";
        return role == "manager";
    }

    public static bool ExceedsAggregateCap(decimal currentApprovedTotal, decimal newAmount, bool hasOverride) =>
        !hasOverride && currentApprovedTotal + newAmount > SeedConstants.AggregateReserveCap;
}
