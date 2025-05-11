using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProcessingOrders.Api.Models;

public class CreateOrderRequest
{
    [Required]
    public long CustomerId { get; set; }
    
    [Required]
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    [Required]
    public string ProductId { get; set; } = null!;
    
    [Required]
    public string ProductName { get; set; } = null!;
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    public string Currency { get; set; } = "RUB";
}

public class UpdateOrderStatusRequest
{
    [Required]
    public string Status { get; set; } = null!;
    
    public string? CancellationReason { get; set; }
} 