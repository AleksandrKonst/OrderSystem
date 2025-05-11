using System.Threading.Tasks;
using ProcessingOrders.CoreDomain.Aggregates;
using ProcessingOrders.CoreDomain.ValueObjects;
using ProcessingOrders.Infrastructure.Services.Promotions;

namespace ProcessingOrders.Application.Services;

public class DiscountService
{
    private readonly IPromotionsService _promotionsService;

    public DiscountService(IPromotionsService promotionsService)
    {
        _promotionsService = promotionsService;
    }

    public async Task ApplyPromotionsToOrder(OrderAggregate orderAggregate)
    {
        if (orderAggregate.Order.HasDiscount())
            return;
        
        var productIds = orderAggregate.GetAllProductIds();
        
        foreach (var productId in productIds)
        {
            var promotion = await _promotionsService.GetActivePromotionForProductAsync(productId);
            
            if (promotion != null)
            {
                var discount = Discount.Create(
                    promotion.Id,
                    promotion.Name,
                    promotion.Description,
                    promotion.DiscountPercentage,
                    promotion.ValidUntil
                );
                
                orderAggregate.ApplyDiscount(discount);
                break;
            }
        }
    }
    
    public async Task<bool> HasActivePromotion(string productId)
    {
        var promotion = await _promotionsService.GetActivePromotionForProductAsync(productId);
        return promotion != null;
    }
} 