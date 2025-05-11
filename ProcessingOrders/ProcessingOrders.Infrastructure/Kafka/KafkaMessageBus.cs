using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.CoreDomain.Events;

namespace ProcessingOrders.Infrastructure.Kafka;

public class KafkaMessageBus : IMessageBus
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaMessageBus> _logger;

    public KafkaMessageBus(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaMessageBus> logger)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = Acks.Leader,
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Reason}", e.Reason))
            .Build();
    }

    public async Task PublishOrderEventAsync<T>(T orderEvent) where T : IDomainEvent
    {
        try
        {
            var topic = orderEvent switch
            {
                OrderCreatedEvent => _settings.OrdersTopic,
                OrderItemAddedEvent => _settings.OrdersTopic,
                OrderItemRemovedEvent => _settings.OrdersTopic,
                OrderStatusChangedEvent => _settings.OrdersTopic,
                OrderCancelledEvent => _settings.OrdersTopic,
                OrderDiscountAppliedEvent => _settings.OrdersDiscountTopic,
                OrderDiscountRemovedEvent => _settings.OrdersDiscountTopic,
                _ => _settings.OrdersTopic
            };

            var key = GetEventKey(orderEvent);
            var message = JsonSerializer.Serialize(orderEvent);

            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = message
            });

            _logger.LogInformation(
                "Отправлено сообщение в Kafka: {Topic}, {Partition}, {Offset}, {Key}",
                result.Topic, result.Partition, result.Offset, key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в Kafka");
        }
    }

    private string GetEventKey<T>(T orderEvent) where T : IDomainEvent
    {
        return $"{orderEvent.GetType().Name}-{Guid.NewGuid()}";
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
} 