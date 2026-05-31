namespace ClaimsModule.Application.Common.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(
        Guid claimId,
        string eventType,
        string description,
        string? oldValue = null,
        string? newValue = null,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        CancellationToken cancellationToken = default);
}
