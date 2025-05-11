using System;

namespace ProcessingOrders.CoreDomain.ValueObjects;

public class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Валюта не может быть пустой", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Create(decimal amount, string currency)
    {
        return new Money(amount, currency);
    }

    public Money Add(Money money)
    {
        if (Currency != money.Currency)
            throw new InvalidOperationException($"Нельзя складывать деньги разных валют: {Currency} и {money.Currency}");

        return new Money(Amount + money.Amount, Currency);
    }

    public Money Subtract(Money money)
    {
        if (Currency != money.Currency)
            throw new InvalidOperationException($"Нельзя вычитать деньги разных валют: {Currency} и {money.Currency}");

        return new Money(Amount - money.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }

    public override string ToString()
    {
        return $"{Amount} {Currency}";
    }

    public bool Equals(Money? other)
    {
        if (other is null)
            throw new ArgumentException("Значение не может быть пустым");
        
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public static bool operator ==(Money left, Money right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Money left, Money right)
    {
        return !(left == right);
    }
} 