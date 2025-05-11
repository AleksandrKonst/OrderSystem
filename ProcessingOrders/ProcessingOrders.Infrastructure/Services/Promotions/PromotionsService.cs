using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessingOrders.Infrastructure.Protos;

namespace ProcessingOrders.Infrastructure.Services.Promotions;

public class PromotionsService : IPromotionsService
{
    private readonly ILogger<PromotionsService> _logger;
    private readonly Protos.Promotions.PromotionsClient _client;

    public PromotionsService(
        IOptions<PromotionsGrpcSettings> settings,
        ILogger<PromotionsService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = new Protos.Promotions.PromotionsClient(GrpcChannel.ForAddress(settings.Value.Address));
    }

    public async Task<PromotionDto?> GetActivePromotionForProductAsync(string productId)
    {
        try
        {
            _logger.LogInformation("Получение акций для продукта {ProductId}", productId);
            
            var request = new GetProductPromotionRequest { ProductId = productId };
            var response = await _client.GetProductPromotionAsync(request);
            
            if (response?.Promotion == null)
            {
                return null;
            }
            
            return new PromotionDto
            {
                Id = response.Promotion.Id,
                Name = response.Promotion.Name,
                Description = response.Promotion.Description,
                DiscountPercentage = (decimal)response.Promotion.DiscountPercentage,
                ValidUntil = DateTime.Parse(response.Promotion.ValidUntil)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении акций для продукта {ProductId}", productId);
            return null;
        }
    }
}

public interface IPromotionsService
{
    Task<PromotionDto?> GetActivePromotionForProductAsync(string productId);
}

public class PromotionDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidUntil { get; set; }
}

public class PromotionsGrpcSettings
{
    public string Address { get; set; } = null!;
} 