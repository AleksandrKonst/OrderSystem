using System;

namespace ProcessingOrders.CoreDomain.ValueObjects;

public class Discount : IEquatable<Discount>
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal Percentage { get; }
    public DateTime ValidUntil { get; }

    private Discount(string id, string name, string description, decimal percentage, DateTime validUntil)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id скидки не может быть пустым", nameof(id));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название скидки не может быть пустым", nameof(name));
            
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Процент скидки должен быть между 0 и 100", nameof(percentage));
            
        Id = id;
        Name = name;
        Description = description;
        Percentage = percentage;
        ValidUntil = validUntil;
    }

    public static Discount Create(string id, string name, string description, decimal percentage, DateTime validUntil)
    {
        return new Discount(id, name, description, percentage, validUntil);
    }

    public bool IsValid()
    {
        return DateTime.UtcNow <= ValidUntil;
    }

    public Money ApplyTo(Money price)
    {
        if (!IsValid())
            throw new InvalidOperationException($"Скидка {Id} уже истекла");

        var discountAmount = price.Amount * (Percentage / 100m);
        var discountedAmount = price.Amount - discountAmount;
        
        return new Money(discountedAmount, price.Currency);
    }

    public bool Equals(Discount? other)
    {
        if (other is null) return false;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Discount left, Discount right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Discount left, Discount right)
    {
        return !(left == right);
    }
} 