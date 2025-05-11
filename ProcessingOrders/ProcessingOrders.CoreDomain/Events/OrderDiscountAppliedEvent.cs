using System;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.CoreDomain.Events;

public class OrderDiscountAppliedEvent : IDomainEvent
{
    public long OrderId { get; }
    public string DiscountId { get; }
    public string DiscountName { get; }
    public decimal DiscountPercentage { get; }
    public Money OriginalAmount { get; }
    public Money DiscountedAmount { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }

    public OrderDiscountAppliedEvent(
        long orderId, 
        string discountId, 
        string discountName, 
        decimal discountPercentage, 
        Money originalAmount, 
        Money discountedAmount,
        DateTime occurredOn)
    {
        OrderId = orderId;
        DiscountId = discountId;
        DiscountName = discountName;
        DiscountPercentage = discountPercentage;
        OriginalAmount = originalAmount;
        DiscountedAmount = discountedAmount;
        EventId = Guid.NewGuid();
        OccurredOn = occurredOn;
    }
} 