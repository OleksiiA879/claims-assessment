using ClaimsModule.Domain.Common;
using MediatR;

namespace ClaimsModule.Domain.Events;

public sealed record ClaimCreatedEvent(
    Guid ClaimId,
    string ClaimNumber,
    Guid OrganisationId,
    Guid UserId) : IDomainEvent, INotification
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
