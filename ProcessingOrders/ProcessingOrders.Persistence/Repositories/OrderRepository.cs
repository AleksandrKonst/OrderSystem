using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProcessingOrders.CoreDomain.Aggregates;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.CoreDomain.Entities;
using ProcessingOrders.CoreDomain.Repositories;
using ProcessingOrders.CoreDomain.ValueObjects;
using ProcessingOrders.Persistence.Context;

namespace ProcessingOrders.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
    private readonly IDomainEventService _domainEventService;

    public OrderRepository(
        OrderDbContext context, 
        IDomainEventService domainEventService)
    {
        _context = context;
        _domainEventService = domainEventService;
    }

    public async Task<OrderAggregate?> GetByIdAsync(long id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order is null)
            return null;

        var items = await _context.OrderItems
            .Where(i => i.OrderId == id)
            .ToListAsync();

        return OrderAggregate.LoadFromStorage(order, items);
    }

    public async Task<List<OrderAggregate>> GetByCustomerIdAsync(CustomerId customerId)
    {
        var customerIdValue = customerId.Value;
        
        var orders = await _context.Orders
            .Where(o => o.CustomerId.Value == customerIdValue)
            .ToListAsync();

        return await LoadOrderAggregates(orders);
    }

    public async Task<List<OrderAggregate>> GetAllAsync()
    {
        var orders = await _context.Orders.ToListAsync();
        return await LoadOrderAggregates(orders);
    }

    public async Task<List<OrderAggregate>> GetByStatusAsync(OrderStatus status)
    {
        var orders = await _context.Orders
            .Where(o => o.Status == status)
            .ToListAsync();

        return await LoadOrderAggregates(orders);
    }

    private async Task<List<OrderAggregate>> LoadOrderAggregates(List<Order> orders)
    {
        var result = new List<OrderAggregate>();

        foreach (var order in orders)
        {
            var orderIdValue = order.Id;
            
            var items = await _context.OrderItems
                .Where(i => i.OrderId == orderIdValue)
                .ToListAsync();

            result.Add(OrderAggregate.LoadFromStorage(order, items));
        }

        return result;
    }

    public async Task AddAsync(OrderAggregate orderAggregate)
    {
        try
        {
            if (orderAggregate.Order.Id != 0)
            {
                throw new InvalidOperationException("Cannot add an order with a non-zero ID");
            }
            
            await _context.Orders.AddAsync(orderAggregate.Order);
            await _context.SaveChangesAsync();
            
            orderAggregate.Id = orderAggregate.Order.Id;
            
            foreach (var item in orderAggregate.Items)
            {
                item.OrderId = orderAggregate.Id;
                item.Id = 0;
                
                await _context.OrderItems.AddAsync(item);
            }
            
            if (orderAggregate.Items.Count > 0)
            {
                await _context.SaveChangesAsync();
            }
            
            await PublishEvents(orderAggregate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddAsync: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    public async Task UpdateAsync(OrderAggregate orderAggregate)
    {
        try
        {
            if (orderAggregate.Id <= 0)
            {
                throw new InvalidOperationException("Cannot update an order with an invalid ID");
            }
            
            _context.Orders.Update(orderAggregate.Order);
            
            var orderIdValue = orderAggregate.Id;
            
            var existingItems = await _context.OrderItems
                .Where(i => i.OrderId == orderIdValue)
                .ToListAsync();
            
            var itemsToRemove = existingItems
                .Where(ei => !orderAggregate.Items.Any(i => i.Id > 0 && i.Id == ei.Id))
                .ToList();
            
            foreach (var item in itemsToRemove)
            {
                _context.OrderItems.Remove(item);
            }
            
            foreach (var item in orderAggregate.Items)
            {
                if (item.Id <= 0)
                {
                    item.OrderId = orderAggregate.Id;
                    
                    await _context.OrderItems.AddAsync(item);
                }
                else
                {
                    // Existing item
                    var existingItem = existingItems.FirstOrDefault(ei => ei.Id == item.Id);
                    if (existingItem is not null)
                    {
                        _context.Entry(existingItem).CurrentValues.SetValues(item);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot update an item with ID {item.Id} that doesn't exist");
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            
            await PublishEvents(orderAggregate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        var order = await _context.Orders.FindAsync(id);
            
        if (order is not null)
        {
            var items = await _context.OrderItems
                .Where(i => i.OrderId == id)
                .ToListAsync();
            
            _context.OrderItems.RemoveRange(items);
            _context.Orders.Remove(order);
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == id);
    }
    
    private async Task PublishEvents(OrderAggregate orderAggregate)
    {
        foreach (var domainEvent in orderAggregate.DomainEvents)
        {
            await _domainEventService.PublishAsync(domainEvent);
        }
        
        orderAggregate.ClearDomainEvents();
    }
} 