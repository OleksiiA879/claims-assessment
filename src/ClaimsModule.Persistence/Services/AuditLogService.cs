using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Persistence.Services;

public class AuditLogService(ClaimsDbContext context, ICurrentUserService currentUser) : IAuditLogService
{
    public async Task LogAsync(
        Guid claimId,
        string eventType,
        string description,
        string? oldValue = null,
        string? newValue = null,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        CancellationToken cancellationToken = default)
    {
        context.ClaimAuditLogs.Add(new ClaimAuditLog
        {
            Id = Guid.NewGuid(),
            ClaimId = claimId,
            OrganisationId = currentUser.OrganisationId,
            EventType = eventType,
            Description = description,
            OldValue = oldValue,
            NewValue = newValue,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            CorrelationId = currentUser.CorrelationId,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByUserId = currentUser.UserId
        });
        await context.SaveChangesAsync(cancellationToken);
    }
}
