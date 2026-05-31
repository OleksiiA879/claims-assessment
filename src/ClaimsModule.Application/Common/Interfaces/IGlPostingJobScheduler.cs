namespace ClaimsModule.Application.Common.Interfaces;

public interface IGlPostingJobScheduler
{
    void Enqueue(Guid reserveHistoryId, Guid claimId, string idempotencyKey);
}
