using System.Threading.Tasks;

namespace ProcessingOrders.CoreDomain.Common;

public interface IDomainEventService
{
    Task PublishAsync(IDomainEvent domainEvent);
} 