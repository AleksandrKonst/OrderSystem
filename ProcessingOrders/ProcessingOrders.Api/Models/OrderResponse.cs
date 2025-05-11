using System;
using System.Collections.Generic;
using System.Linq;
using ProcessingOrders.CoreDomain.Aggregates;
using ProcessingOrders.CoreDomain.Entities;

namespace ProcessingOrders.Api.Models;

public class OrderResponse
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string Status { get; set; } = null!;
    public List<OrderItemResponse> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = null!;
    public decimal? OriginalAmount { get; set; }
    public DiscountResponse? AppliedDiscount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static OrderResponse FromAggregate(OrderAggregate orderAggregate)
    {
        var order = orderAggregate.Order;
        
        var response = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId.Value,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount.Amount,
            Currency = order.TotalAmount.Currency,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = orderAggregate.Items.Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price.Amount,
                Currency = item.Price.Currency
            }).ToList()
        };
        
        if (order.HasDiscount() && order.AppliedDiscount is not null && order.OriginalAmount is not null)
        {
            response.OriginalAmount = order.OriginalAmount.Amount;
            response.AppliedDiscount = new DiscountResponse
            {
                Id = order.AppliedDiscount.Id,
                Name = order.AppliedDiscount.Name,
                Description = order.AppliedDiscount.Description,
                DiscountPercentage = order.AppliedDiscount.Percentage,
                ValidUntil = order.AppliedDiscount.ValidUntil
            };
        }

        return response;
    }

    public static OrderResponse FromDomain(Order order)
    {
        var response = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId.Value,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount.Amount,
            Currency = order.TotalAmount.Currency,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = new List<OrderItemResponse>()
        };
        
        if (order.HasDiscount() && order.AppliedDiscount is not null && order.OriginalAmount is not null)
        {
            response.OriginalAmount = order.OriginalAmount.Amount;
            response.AppliedDiscount = new DiscountResponse
            {
                Id = order.AppliedDiscount.Id,
                Name = order.AppliedDiscount.Name,
                Description = order.AppliedDiscount.Description,
                DiscountPercentage = order.AppliedDiscount.Percentage,
                ValidUntil = order.AppliedDiscount.ValidUntil
            };
        }

        return response;
    }
}

public class OrderItemResponse
{
    public long Id { get; set; }
    public string ProductId { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = null!;
}

public class DiscountResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = "";
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidUntil { get; set; }
} 