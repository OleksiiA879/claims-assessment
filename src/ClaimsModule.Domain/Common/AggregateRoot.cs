namespace ClaimsModule.Domain.Common;

public abstract class AggregateRoot : AuditableEntity
{
    public byte[] RowVer { get; set; } = [];
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
