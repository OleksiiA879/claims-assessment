using ClaimsModule.Domain.Common;

namespace ClaimsModule.Domain.Events;

public sealed record ClaimCreatedEvent(
    Guid ClaimId,
    string ClaimNumber,
    Guid OrganisationId,
    Guid UserId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
