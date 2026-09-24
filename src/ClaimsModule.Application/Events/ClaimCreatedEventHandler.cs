using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Common.Models;
using ClaimsModule.Domain.Events;
using MediatR;

namespace ClaimsModule.Application.Events;

public class ClaimCreatedEventHandler(IAuditLogService auditLog)
    : INotificationHandler<DomainEventNotification<ClaimCreatedEvent>>
{
    public Task Handle(DomainEventNotification<ClaimCreatedEvent> notification, CancellationToken cancellationToken) =>
        auditLog.LogAsync(notification.DomainEvent.ClaimId, "CLAIM_CREATED",
            $"Domain event: claim {notification.DomainEvent.ClaimNumber} created.",
            cancellationToken: cancellationToken);
}
