using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProcessingOrders.Application.Features.Orders.Commands.ApplyDiscount;
using ProcessingOrders.Application.Services;
using System.Threading.Tasks;

namespace ProcessingOrders.Api.Endpoints;

public static class DiscountEndpoints
{
    public static RouteGroupBuilder MapDiscountEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{orderId}", ApplyDiscountToOrder)
            .WithName("ApplyDiscountToOrder")
            .WithDisplayName("Применить скидку к заказу");
        
        group.MapGet("/product/{productId}", CheckProductPromotion)
            .WithName("CheckProductPromotion")
            .WithDisplayName("Проверить наличие акций для продукта");
        
        return group;
    }
    
    private static async Task<IResult> ApplyDiscountToOrder(
        long orderId, 
        ApplyDiscountRequest request, 
        IMediator mediator)
    {
        var command = new ApplyDiscountCommand
        {
            OrderId = orderId,
            DiscountId = request.DiscountId,
            DiscountName = request.DiscountName,
            DiscountDescription = request.DiscountDescription,
            DiscountPercentage = request.DiscountPercentage,
            ValidUntil = request.ValidUntil
        };
        
        var result = await mediator.Send(command);
        
        return Results.Ok(result);
    }
    
    private static async Task<IResult> CheckProductPromotion(
        string productId,
        DiscountService discountService)
    {
        var hasPromotion = await discountService.HasActivePromotion(productId);
        
        return Results.Ok(new { HasPromotion = hasPromotion });
    }
}

public class ApplyDiscountRequest
{
    public string DiscountId { get; set; } = null!;
    public string DiscountName { get; set; } = null!;
    public string DiscountDescription { get; set; } = "";
    public decimal DiscountPercentage { get; set; }
    public System.DateTime ValidUntil { get; set; }
} 