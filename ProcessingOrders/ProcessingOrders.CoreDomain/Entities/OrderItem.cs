using System;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.CoreDomain.Entities;

public sealed class OrderItem : Entity<long>
{
    public long OrderId { get; set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }

    public static OrderItem Create(
        long orderId,
        string productId,
        string productName,
        int quantity,
        Money price)
    {
        if (orderId <= 0)
            throw new ArgumentException("OrderId должен быть положительным числом", nameof(orderId));

        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("ProductId не может быть пустым", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("ProductName не может быть пустым", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля", nameof(quantity));

        if (price is null)
            throw new ArgumentNullException(nameof(price), "Цена не может быть null");

        if (price.Amount <= 0)
            throw new ArgumentException("Цена должна быть больше нуля", nameof(price));
        
        return new OrderItem()
        {
            Id = 0,
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            Price = price
        };
    }
}