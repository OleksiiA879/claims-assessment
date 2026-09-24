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
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var claimed = false;
        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var updated = await context.ReserveHistories
                .Where(h => h.Id == reserveHistoryId &&
                            h.ClaimId == claimId &&
                            h.IdempotencyKey == idempotencyKey &&
                            h.PostingStatus != ReservePostingStatus.Posted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(h => h.PostingStatus, ReservePostingStatus.Posted)
                    .SetProperty(h => h.UpdatedAt, DateTimeOffset.UtcNow), ct);

            if (updated == 0)
                return;

            var amount = await context.ReserveHistories
                .Where(h => h.Id == reserveHistoryId)
                .Select(h => h.Amount)
                .SingleAsync(ct);
            var journal = $"DR Change in Outstanding Reserves / CR Outstanding Loss Reserves | Amount={amount:F4}";
            await auditLog.LogAsync(claimId, "GL_POSTING_SIMULATED", journal,
                newValue: journal, relatedEntityId: reserveHistoryId, relatedEntityType: "ReserveHistory",
                cancellationToken: ct);
            await unitOfWork.SaveChangesAsync(ct);
            claimed = true;
        });

        if (claimed)
            logger.LogInformation("GL posting simulated for {Key}", idempotencyKey);
        else
            logger.LogInformation("GL posting already completed or missing for {Key}; idempotent no-op", idempotencyKey);
    }
}
