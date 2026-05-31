using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Events;
using MediatR;

namespace ClaimsModule.Application.Events;

public class ClaimCreatedEventHandler(IAuditLogService auditLog) : INotificationHandler<ClaimCreatedEvent>
{
    public Task Handle(ClaimCreatedEvent notification, CancellationToken cancellationToken) =>
        auditLog.LogAsync(notification.ClaimId, "CLAIM_CREATED",
            $"Domain event: claim {notification.ClaimNumber} created.",
            cancellationToken: cancellationToken);
}
