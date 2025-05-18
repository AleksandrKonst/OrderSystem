using System.Collections.Generic;

namespace ProcessingOrders.CoreDomain.Common;

public interface IAggregate
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void AddDomainEvent(IDomainEvent domainEvent);
    IDomainEvent[] ClearDomainEvents();
} 