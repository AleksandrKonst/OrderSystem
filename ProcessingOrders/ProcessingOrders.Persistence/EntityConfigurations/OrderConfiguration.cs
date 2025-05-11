using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessingOrders.CoreDomain.Entities;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.Persistence.EntityConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(o => o.CustomerId)
            .HasConversion(
                id => id.Value,
                value => CustomerId.FromLong(value))
            .IsRequired();
            
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(o => o.TotalAmount, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("TotalAmount")
                .IsRequired();
                
            m.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        
        builder.OwnsOne(o => o.OriginalAmount, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("OriginalAmount");
                
            m.Property(p => p.Currency)
                .HasColumnName("OriginalCurrency")
                .HasMaxLength(3);
        });
        
        builder.OwnsOne(o => o.AppliedDiscount, d =>
        {
            d.Property(p => p.Id)
                .HasColumnName("DiscountId")
                .HasMaxLength(50);
                
            d.Property(p => p.Name)
                .HasColumnName("DiscountName")
                .HasMaxLength(100);
                
            d.Property(p => p.Description)
                .HasColumnName("DiscountDescription")
                .HasMaxLength(500);
                
            d.Property(p => p.Percentage)
                .HasColumnName("DiscountPercentage")
                .HasPrecision(10, 2);
                
            d.Property(p => p.ValidUntil)
                .HasColumnName("DiscountValidUntil")
                .HasColumnType("timestamp with time zone");
        });

        builder.Property(o => o.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .HasColumnType("timestamp with time zone");
    }
} 