namespace ClaimsModule.Domain.Constants;

public static class SeedConstants
{
    public static readonly Guid DefaultOrganisationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid HandlerUserId = Guid.Parse("22222222-2222-2222-2222-222222222001");
    public static readonly Guid SupervisorUserId = Guid.Parse("22222222-2222-2222-2222-222222222002");
    public static readonly Guid ManagerUserId = Guid.Parse("22222222-2222-2222-2222-222222222003");

    public const decimal AutoApprovalThreshold = 10_000m;
    public const decimal SupervisorApprovalThreshold = 100_000m;
    public const decimal AggregateReserveCap = 10_000_000m;
}
