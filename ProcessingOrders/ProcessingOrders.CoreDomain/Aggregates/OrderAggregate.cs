using System;
using System.Collections.Generic;
using System.Linq;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.CoreDomain.Entities;
using ProcessingOrders.CoreDomain.Events;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.CoreDomain.Aggregates;

public class OrderAggregate : Aggregate<long>
{
    public required Order Order { get; set; }
    public List<OrderItem> Items => new();

    public static OrderAggregate Create(CustomerId customerId, List<OrderItemData> itemsData)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId), "CustomerId не может быть null");

        if (itemsData == null || itemsData.Count == 0)
            throw new ArgumentException("Заказ должен содержать хотя бы один товар", nameof(itemsData));


        var defaultCurrency = itemsData.FirstOrDefault()?.Currency ?? "RUB";
        var initialTotalAmount = new Money(0, defaultCurrency);
        
        var order = Order.Create(
            0,
            customerId,
            OrderStatus.Created,
            initialTotalAmount,
            DateTime.UtcNow
        );

        var aggregate = new OrderAggregate
        {
            Id = 0,
            Order = order
        };
        
        return aggregate;
    }
    
    public void AddItemsAfterSave(List<OrderItemData> itemsData)
    {
        if (Id <= 0)
        {
            throw new InvalidOperationException("Order must be saved to database first to get an ID");
        }
        
        System.Console.WriteLine($"Adding items to order with ID: {Id}");
        
        var items = new List<OrderItem>();
        foreach (var data in itemsData)
        {
            var item = OrderItem.Create(
                Id,
                data.ProductId,
                data.ProductName,
                data.Quantity,
                new Money(data.Price, data.Currency)
            );
            items.Add(item);
        }
        
        foreach (var item in items)
        {
            Items.Add(item);
        }
        
        var totalAmount = CalculateTotalAmount(items);
        Order.UpdateTotalAmount(totalAmount);
        
        AddDomainEvent(new OrderCreatedEvent(Id, Order.CustomerId, totalAmount));
    }
    
    public static OrderAggregate LoadFromStorage(Order order, List<OrderItem> items)
    {
        var aggregate = new OrderAggregate
        {
            Id = order.Id,
            Order = order
        };

        foreach (var item in items)
        {
            aggregate.Items.Add(item);
        }

        return aggregate;
    }

    private static Money CalculateTotalAmount(List<OrderItem> items)
    {
        if (items.Count == 0)
            return new Money(0, "RUB"); // Default currency if no items
            
        var total = items.Sum(item => item.Price.Amount * item.Quantity);
            
        var currency = items[0].Price.Currency;
        return new Money(total, currency);
    }

    public void AddItem(string productId, string productName, int quantity, decimal price, string currency)
    {
        if (Order.Status != OrderStatus.Created)
            throw new InvalidOperationException("Нельзя добавлять товары к заказу в статусе " + Order.Status);

        var item = OrderItem.Create(
            Id,
            productId,
            productName,
            quantity,
            new Money(price, currency)
        );
            
        Items.Add(item);
        RecalculateTotalAmount();
            
        AddDomainEvent(new OrderItemAddedEvent(Id, item));
    }

    public void RemoveItem(long itemId)
    {
        if (Order.Status != OrderStatus.Created)
            throw new InvalidOperationException("Нельзя удалять товары из заказа в статусе " + Order.Status);

        var itemToRemove = Items.Find(i => i.Id == itemId);
        if (itemToRemove is null)
            throw new ArgumentException($"Товар с id {itemId} не найден в заказе", nameof(itemId));

        Items.Remove(itemToRemove);
        RecalculateTotalAmount();
            
        AddDomainEvent(new OrderItemRemovedEvent(Id, itemId));
    }

    private void RecalculateTotalAmount()
    {
        var totalAmount = CalculateTotalAmount(Items);
        
        var hasDiscount = Order.HasDiscount();
        var discount = Order.AppliedDiscount;
        
        if (hasDiscount)
        {
            Order.RemoveDiscount();
        }
        
        Order.UpdateTotalAmount(totalAmount);

        if (hasDiscount && discount is not null)
        {
            ApplyDiscount(discount);
        }
    }
    
    public void ApplyDiscount(Discount discount)
    {
        if (Order.Status != OrderStatus.Created && Order.Status != OrderStatus.Processing)
            throw new InvalidOperationException($"Нельзя применить скидку к заказу в статусе {Order.Status}");
            
        if (!discount.IsValid())
            throw new InvalidOperationException($"Скидка {discount.Id} недействительна");
            
        var originalAmount = Order.TotalAmount;
        Order.ApplyDiscount(discount);
        
        AddDomainEvent(new OrderDiscountAppliedEvent(
            Id, 
            discount.Id, 
            discount.Name, 
            discount.Percentage, 
            originalAmount, 
            Order.TotalAmount,
            DateTime.UtcNow));
    }
    
    public void RemoveDiscount()
    {
        if (!Order.HasDiscount())
            return;
            
        var discount = Order.AppliedDiscount;
        var discountedAmount = Order.TotalAmount;
        
        Order.RemoveDiscount();
        
        if (discount is not null)
        {
            AddDomainEvent(new OrderDiscountRemovedEvent(
                Id, 
                discount.Id, 
                Order.TotalAmount, 
                discountedAmount,
                DateTime.UtcNow));
        }
    }
    
    public bool CanApplyProductDiscount(string productId)
    {
        return Items.Any(i => i.ProductId == productId);
    }
    
    public List<string> GetAllProductIds()
    {
        return Items.Select(i => i.ProductId).Distinct().ToList();
    }

    public void ProcessOrder()
    {
        if (Order.Status != OrderStatus.Created)
            throw new InvalidOperationException($"Заказ должен быть в статусе Created, текущий статус: {Order.Status}");

        var previousStatus = Order.Status;
        Order.UpdateStatus(OrderStatus.Processing);
            
        AddDomainEvent(new OrderStatusChangedEvent(Id, previousStatus, OrderStatus.Processing));
    }

    public void CompleteOrder()
    {
        if (Order.Status != OrderStatus.Processing)
            throw new InvalidOperationException($"Заказ должен быть в статусе Processing, текущий статус: {Order.Status}");

        var previousStatus = Order.Status;
        Order.UpdateStatus(OrderStatus.Completed);
            
        AddDomainEvent(new OrderStatusChangedEvent(Id, previousStatus, OrderStatus.Completed));
    }

    public void CancelOrder(string reason)
    {
        if (Order.Status is OrderStatus.Cancelled or OrderStatus.Completed)
            throw new InvalidOperationException($"Нельзя отменить заказ в статусе {Order.Status}");

        var previousStatus = Order.Status;
        Order.UpdateStatus(OrderStatus.Cancelled);
            
        AddDomainEvent(new OrderCancelledEvent(Id, previousStatus, reason));
    }
}

public class OrderItemData
{
    public string ProductId { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "RUB";
}