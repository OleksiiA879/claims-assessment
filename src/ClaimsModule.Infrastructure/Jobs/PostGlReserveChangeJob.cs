using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClaimsModule.Infrastructure.Jobs;

public class PostGlReserveChangeJob(IServiceScopeFactory scopeFactory, ILogger<PostGlReserveChangeJob> logger)
{
    public async Task ExecuteAsync(Guid reserveHistoryId, Guid claimId, string idempotencyKey)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var auditLog = scope.ServiceProvider.GetRequiredService<IAuditLogService>();

        var history = await context.ReserveHistories
            .FirstOrDefaultAsync(h => h.Id == reserveHistoryId && h.IdempotencyKey == idempotencyKey);

        if (history is null)
        {
            logger.LogWarning("Reserve history {Id} not found for GL posting", reserveHistoryId);
            return;
        }

        if (history.PostingStatus == ReservePostingStatus.Posted)
        {
            logger.LogInformation("GL posting already completed for {Key} — idempotent no-op", idempotencyKey);
            return;
        }

        var journal = $"DR Change in Outstanding Reserves / CR Outstanding Loss Reserves | Amount={history.Amount:F4}";
        await auditLog.LogAsync(claimId, "GL_POSTING_SIMULATED", journal,
            newValue: journal, relatedEntityId: history.Id, relatedEntityType: "ReserveHistory");

        history.PostingStatus = ReservePostingStatus.Posted;
        await context.SaveChangesAsync();
        logger.LogInformation("GL posting simulated for {Key}", idempotencyKey);
    }
}
