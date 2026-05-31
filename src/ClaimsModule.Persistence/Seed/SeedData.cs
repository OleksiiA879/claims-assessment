using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Seed;

public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        var orgId = SeedConstants.DefaultOrganisationId;

        modelBuilder.Entity<CauseOfLossCode>().HasData(
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000001"), "COL-FIRE", "Fire", "Property", 1),
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000002"), "COL-FLOOD", "Flood", "Weather", 2),
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000003"), "COL-THEFT", "Theft", "Crime", 3),
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000004"), "COL-VEH-COL", "Vehicle Collision", "Auto", 4),
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000005"), "COL-VEH-COMP", "Vehicle Comprehensive", "Auto", 5),
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000006"), "COL-LIAB", "Third Party Liability", "Liability", 6),
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000007"), "COL-EQUIP", "Equipment Breakdown", "Equipment", 7),
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000008"), "COL-WIND", "Wind / Storm", "Weather", 8),
            Cause(orgId, new Guid("30000001-0001-0001-0001-000000000009"), "COL-INJURY", "Bodily Injury", "Liability", 9),
            Cause(orgId, new Guid("30000001-0001-0001-0001-00000000000a"), "COL-OTHER", "Other / Unknown", "General", 10));

        modelBuilder.Entity<Policy>().HasData(
            Policy(new Guid("40000001-0001-0001-0001-000000000001"), orgId, "POL-2024-001001", "Meridian Transport LLC", new(2024, 1, 1), new(2026, 12, 31), "Active", "Vehicle,Cargo"),
            Policy(new Guid("40000001-0001-0001-0001-000000000002"), orgId, "POL-2024-001002", "Harborview Properties Inc", new(2024, 6, 1), new(2026, 5, 31), "Active", "Property,Liability"),
            Policy(new Guid("40000001-0001-0001-0001-000000000003"), orgId, "POL-2025-002001", "Coastal Builders Group", new(2025, 3, 1), new(2027, 2, 28), "Active", "Property,Equipment"),
            Policy(new Guid("40000001-0001-0001-0001-000000000004"), orgId, "POL-2025-002002", "Stanton Medical Group", new(2025, 1, 1), new(2026, 12, 31), "Active", "Liability,Vehicle"),
            Policy(new Guid("40000001-0001-0001-0001-000000000005"), orgId, "POL-2023-000099", "Archived Corp", new(2020, 1, 1), new(2021, 12, 31), "Expired", "Property"));

        modelBuilder.Entity<ClaimStatusTransition>().HasData(
            Transition(new Guid("50000001-0001-0001-0001-000000000001"), ClaimStatus.Draft, ClaimStatus.Open),
            Transition(new Guid("50000001-0001-0001-0001-000000000002"), ClaimStatus.Open, ClaimStatus.UnderInvestigation),
            Transition(new Guid("50000001-0001-0001-0001-000000000003"), ClaimStatus.Open, ClaimStatus.PendingPayment),
            Transition(new Guid("50000001-0001-0001-0001-000000000004"), ClaimStatus.Open, ClaimStatus.Closed),
            Transition(new Guid("50000001-0001-0001-0001-000000000005"), ClaimStatus.Open, ClaimStatus.Withdrawn),
            Transition(new Guid("50000001-0001-0001-0001-000000000006"), ClaimStatus.UnderInvestigation, ClaimStatus.Open),
            Transition(new Guid("50000001-0001-0001-0001-000000000007"), ClaimStatus.UnderInvestigation, ClaimStatus.PendingPayment),
            Transition(new Guid("50000001-0001-0001-0001-000000000008"), ClaimStatus.UnderInvestigation, ClaimStatus.Closed),
            Transition(new Guid("50000001-0001-0001-0001-000000000009"), ClaimStatus.UnderInvestigation, ClaimStatus.Withdrawn),
            Transition(new Guid("50000001-0001-0001-0001-00000000000a"), ClaimStatus.PendingPayment, ClaimStatus.Closed),
            Transition(new Guid("50000001-0001-0001-0001-00000000000b"), ClaimStatus.Closed, ClaimStatus.Reopened, "supervisor"),
            Transition(new Guid("50000001-0001-0001-0001-00000000000c"), ClaimStatus.Reopened, ClaimStatus.Open));
    }

    private static CauseOfLossCode Cause(Guid orgId, Guid id, string code, string name, string category, int sort) =>
        new() { Id = id, OrganisationId = orgId, Code = code, Name = name, PerilCategory = category, IsActive = true, SortOrder = sort };

    private static Policy Policy(Guid id, Guid orgId, string number, string client, DateOnly from, DateOnly to, string status, string coverages) =>
        new() { Id = id, OrganisationId = orgId, PolicyNumber = number, ClientName = client, EffectiveDate = from, ExpirationDate = to, Status = status, CoverageTypes = coverages };

    private static ClaimStatusTransition Transition(Guid id, ClaimStatus from, ClaimStatus to, string? permission = null) =>
        new() { Id = id, FromStatus = from, ToStatus = to, RequiredPermission = permission };
}
