using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProcessingOrders.CoreDomain.Entities;
using ProcessingOrders.CoreDomain.Repositories;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.Application.Features.Orders.Commands.ApplyDiscount;

public class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand, Order>
{
    private readonly IOrderRepository _orderRepository;

    public ApplyDiscountCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<Order> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
    {
        var orderAggregate = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (orderAggregate is null)
            throw new ArgumentException($"Заказ с id {request.OrderId} не найден");
            
        if (orderAggregate.Order.Status is OrderStatus.Cancelled or OrderStatus.Completed)
            throw new InvalidOperationException($"Нельзя применить скидку к заказу в статусе {orderAggregate.Order.Status}");
        
        var discount = Discount.Create(
            request.DiscountId,
            request.DiscountName,
            request.DiscountDescription,
            request.DiscountPercentage,
            request.ValidUntil
        );
        
        orderAggregate.ApplyDiscount(discount);
        
        await _orderRepository.UpdateAsync(orderAggregate);
        
        return orderAggregate.Order;
    }
} 