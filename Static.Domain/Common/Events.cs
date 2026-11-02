using System.ComponentModel.DataAnnotations.Schema;

namespace Static.Domain.Common;

public abstract class Events
{

    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainBaseEvent)
    {
        _domainEvents.Add(domainBaseEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainBaseEvent)
    {
        _domainEvents.Remove(domainBaseEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
