using System.Collections.Generic;
using MediatR;
using ProcessingOrders.CoreDomain.Entities;

namespace ProcessingOrders.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Order>
{
    public long CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string ProductId { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "RUB";
} 