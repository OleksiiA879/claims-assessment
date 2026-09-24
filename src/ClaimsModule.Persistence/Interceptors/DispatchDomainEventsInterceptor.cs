using ClaimsModule.Domain.Common;
using ClaimsModule.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClaimsModule.Persistence.Interceptors;

public class DispatchDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is ClaimsDbContext context)
        {
            var aggregates = context.ChangeTracker.Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
            aggregates.ForEach(a => a.ClearDomainEvents());

            foreach (var domainEvent in events)
            {
                var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                var notification = Activator.CreateInstance(notificationType, domainEvent)
                    ?? throw new InvalidOperationException($"Could not wrap domain event {domainEvent.GetType().Name}.");
                await mediator.Publish(notification, cancellationToken);
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
