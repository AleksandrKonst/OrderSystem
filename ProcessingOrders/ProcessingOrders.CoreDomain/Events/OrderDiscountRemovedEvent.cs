using System;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.CoreDomain.Events;

public class OrderDiscountRemovedEvent : IDomainEvent
{
    public long OrderId { get; }
    public string DiscountId { get; }
    public Money OriginalAmount { get; }
    public Money DiscountedAmount { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }

    public OrderDiscountRemovedEvent(
        long orderId, 
        string discountId, 
        Money originalAmount, 
        Money discountedAmount,
        DateTime occurredOn)
    {
        OrderId = orderId;
        DiscountId = discountId;
        OriginalAmount = originalAmount;
        DiscountedAmount = discountedAmount;
        EventId = Guid.NewGuid();
        OccurredOn = occurredOn;
    }
} 