using ClaimsModule.Domain.Common;
using MediatR;

namespace ClaimsModule.Application.Common.Models;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
