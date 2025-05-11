using MediatR;
using ProcessingOrders.CoreDomain.Entities;

namespace ProcessingOrders.Application.Features.Orders.Commands.ApplyDiscount;

public class ApplyDiscountCommand : IRequest<Order>
{
    public long OrderId { get; set; }
    public string DiscountId { get; set; } = null!;
    public string DiscountName { get; set; } = null!;
    public string DiscountDescription { get; set; } = "";
    public decimal DiscountPercentage { get; set; }
    public System.DateTime ValidUntil { get; set; }
} 