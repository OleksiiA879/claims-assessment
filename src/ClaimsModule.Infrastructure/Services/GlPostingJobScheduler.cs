using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Infrastructure.Jobs;
using Hangfire;

namespace ClaimsModule.Infrastructure.Services;

public class GlPostingJobScheduler : IGlPostingJobScheduler
{
    public void Enqueue(Guid reserveHistoryId, Guid claimId, string idempotencyKey) =>
        BackgroundJob.Enqueue<PostGlReserveChangeJob>(j =>
            j.ExecuteAsync(reserveHistoryId, claimId, idempotencyKey));
}
