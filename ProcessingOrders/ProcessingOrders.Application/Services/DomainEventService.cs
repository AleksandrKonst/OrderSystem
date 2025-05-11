using System;
using System.Threading.Tasks;
using MediatR;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.Infrastructure.Kafka;

namespace ProcessingOrders.Application.Services;

public class DomainEventService : IDomainEventService
{
    private readonly IMessageBus _messageBus;

    public DomainEventService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task PublishAsync(IDomainEvent domainEvent)
    {
        await _messageBus.PublishOrderEventAsync(domainEvent);
    }
} 