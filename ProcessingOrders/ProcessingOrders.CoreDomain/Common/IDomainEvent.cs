using System;

namespace ProcessingOrders.CoreDomain.Common;

public interface IDomainEvent : IEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
} 