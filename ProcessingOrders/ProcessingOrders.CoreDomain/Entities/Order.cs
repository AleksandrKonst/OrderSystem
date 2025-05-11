using System;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.CoreDomain.Entities;

public sealed class Order : Entity<long>
{
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public Money? OriginalAmount { get; private set; }
    public Discount? AppliedDiscount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Order Create(
        long? id,
        CustomerId customerId,
        OrderStatus status,
        Money totalAmount,
        DateTime createdAt,
        DateTime? updatedAt = null,
        Discount? appliedDiscount = null,
        Money? originalAmount = null)
    {
        return new Order
        {
            Id = id ?? 0,
            CustomerId = customerId,
            Status = status,
            TotalAmount = totalAmount,
            OriginalAmount = originalAmount,
            AppliedDiscount = appliedDiscount,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
        
    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTotalAmount(Money totalAmount)
    {
        TotalAmount = totalAmount;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ApplyDiscount(Discount discount)
    {
        if (!discount.IsValid())
            throw new InvalidOperationException($"Скидка {discount.Id} недействительна");
        
        OriginalAmount = TotalAmount;
        TotalAmount = discount.ApplyTo(TotalAmount);
        AppliedDiscount = discount;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void RemoveDiscount()
    {
        if (AppliedDiscount is null || OriginalAmount is null)
            return;
        
        TotalAmount = OriginalAmount;
        AppliedDiscount = null;
        OriginalAmount = null;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool HasDiscount()
    {
        return AppliedDiscount is not null;
    }
}