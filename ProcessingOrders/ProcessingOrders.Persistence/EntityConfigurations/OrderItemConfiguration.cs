using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessingOrders.CoreDomain.Entities;

namespace ProcessingOrders.Persistence.EntityConfigurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(o => o.OrderId)
            .IsRequired();

        builder.Property(o => o.ProductId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.ProductName)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(o => o.Quantity)
            .IsRequired();
            
        builder.OwnsOne(o => o.Price, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("Price")
                .IsRequired();
                
            m.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
} 