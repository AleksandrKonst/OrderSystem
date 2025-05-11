using System;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.CoreDomain.Entities;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.CoreDomain.Events;

public abstract class OrderEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public long OrderId { get; }

    protected OrderEvent(long orderId)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        OrderId = orderId;
    }
}

public class OrderCreatedEvent : OrderEvent
{
    public CustomerId CustomerId { get; }
    public Money TotalAmount { get; }

    public OrderCreatedEvent(long orderId, CustomerId customerId, Money totalAmount)
        : base(orderId)
    {
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }
}

public class OrderStatusChangedEvent : OrderEvent
{
    public OrderStatus PreviousStatus { get; }
    public OrderStatus NewStatus { get; }

    public OrderStatusChangedEvent(long orderId, OrderStatus previousStatus, OrderStatus newStatus)
        : base(orderId)
    {
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
    }
}

public class OrderItemAddedEvent : OrderEvent
{
    public OrderItem Item { get; }

    public OrderItemAddedEvent(long orderId, OrderItem item)
        : base(orderId)
    {
        Item = item;
    }
}

public class OrderItemRemovedEvent : OrderEvent
{
    public long ItemId { get; }

    public OrderItemRemovedEvent(long orderId, long itemId)
        : base(orderId)
    {
        ItemId = itemId;
    }
}

public class OrderCancelledEvent : OrderEvent
{
    public OrderStatus PreviousStatus { get; }
    public string Reason { get; }

    public OrderCancelledEvent(long orderId, OrderStatus previousStatus, string reason)
        : base(orderId)
    {
        PreviousStatus = previousStatus;
        Reason = reason;
    }
} 