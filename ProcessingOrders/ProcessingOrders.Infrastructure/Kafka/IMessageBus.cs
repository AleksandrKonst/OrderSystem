using System;
using System.Threading.Tasks;
using ProcessingOrders.CoreDomain.Common;

namespace ProcessingOrders.Infrastructure.Kafka;

public interface IMessageBus : IDisposable
{
    Task PublishOrderEventAsync<T>(T orderEvent) where T : IDomainEvent;
} 