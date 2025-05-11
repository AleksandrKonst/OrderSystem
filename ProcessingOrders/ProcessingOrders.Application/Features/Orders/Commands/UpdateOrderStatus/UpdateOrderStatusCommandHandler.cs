using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProcessingOrders.CoreDomain.Entities;
using ProcessingOrders.CoreDomain.Repositories;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Order>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<Order> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var orderAggregate = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (orderAggregate is null)
            throw new ArgumentException($"Заказ с id {request.OrderId} не найден");

        switch (request.NewStatus)
        {
            case OrderStatus.Processing:
                orderAggregate.ProcessOrder();
                break;
            case OrderStatus.Completed:
                orderAggregate.CompleteOrder();
                break;
            case OrderStatus.Cancelled:
                if (string.IsNullOrWhiteSpace(request.CancellationReason))
                    throw new ArgumentException("При отмене заказа требуется указать причину");
                
                orderAggregate.CancelOrder(request.CancellationReason);
                break;
            case OrderStatus.Created:
            default:
                throw new ArgumentException($"Недопустимый статус заказа: {request.NewStatus}");
        }

        await _orderRepository.UpdateAsync(orderAggregate);

        return orderAggregate.Order;
    }
} 