namespace ProcessingOrders.Infrastructure.Kafka;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string OrdersTopic { get; set; } = "orders";
    public string OrdersDiscountTopic { get; set; } = "orders-discounts";
    public string GroupId { get; set; } = "processing-orders";
} 