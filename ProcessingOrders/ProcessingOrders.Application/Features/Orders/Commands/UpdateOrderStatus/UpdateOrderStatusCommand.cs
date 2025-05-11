using MediatR;
using ProcessingOrders.CoreDomain.Entities;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<Order>
{
    public long OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
    public string? CancellationReason { get; set; }
} 