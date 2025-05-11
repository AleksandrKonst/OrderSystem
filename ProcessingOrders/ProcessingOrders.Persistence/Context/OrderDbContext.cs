using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProcessingOrders.CoreDomain.Entities;
using ProcessingOrders.Persistence.EntityConfigurations;

namespace ProcessingOrders.Persistence.Context;

public class OrderDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
} 