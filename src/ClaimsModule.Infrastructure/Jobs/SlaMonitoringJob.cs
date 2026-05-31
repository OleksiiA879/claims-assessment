using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClaimsModule.Infrastructure.Jobs;

public class SlaMonitoringJob(IServiceScopeFactory scopeFactory, ILogger<SlaMonitoringJob> logger)
{
    public async Task ExecuteAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var auditLog = scope.ServiceProvider.GetRequiredService<IAuditLogService>();

        var threshold = DateTimeOffset.UtcNow.AddHours(-48);
        var staleClaims = await context.Claims
            .Where(c => (c.Status == ClaimStatus.Draft || c.Status == ClaimStatus.Open)
                        && (c.UpdatedAt ?? c.CreatedAt) < threshold)
            .Select(c => c.Id)
            .ToListAsync();

        foreach (var claimId in staleClaims)
        {
            var lastBreach = await context.ClaimAuditLogs
                .Where(a => a.ClaimId == claimId && a.EventType == "SLA_BREACH_DETECTED")
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastBreach != default && lastBreach > DateTimeOffset.UtcNow.AddHours(-24))
                continue;

            await auditLog.LogAsync(claimId, "SLA_BREACH_DETECTED",
                "Claim has not been updated in 48 hours.");
            logger.LogInformation("SLA breach logged for claim {ClaimId}", claimId);
        }
    }
}
