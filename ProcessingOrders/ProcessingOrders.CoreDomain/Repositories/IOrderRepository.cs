using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessingOrders.CoreDomain.Aggregates;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.CoreDomain.Repositories;

public interface IOrderRepository
{
    Task<OrderAggregate?> GetByIdAsync(long id);
    Task<List<OrderAggregate>> GetByCustomerIdAsync(CustomerId customerId);
    Task<List<OrderAggregate>> GetAllAsync();
    Task<List<OrderAggregate>> GetByStatusAsync(OrderStatus status);
    Task AddAsync(OrderAggregate orderAggregate);
    Task UpdateAsync(OrderAggregate orderAggregate);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}