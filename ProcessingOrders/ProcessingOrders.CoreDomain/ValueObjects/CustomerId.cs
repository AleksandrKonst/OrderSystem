using System;

namespace ProcessingOrders.CoreDomain.ValueObjects;

public class CustomerId : IEquatable<CustomerId>
{
    public long Value { get; }
    private static long _counter;

    private CustomerId(long value)
    {
        Value = value;
    }

    public static CustomerId Create()
    {
        return new CustomerId(++_counter);
    }

    public static CustomerId FromLong(long value)
    {
        return new CustomerId(value);
    }

    public static CustomerId FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CustomerId не может быть пустым", nameof(value));

        if (!long.TryParse(value, out var id))
            throw new ArgumentException("CustomerId должен быть валидным числом", nameof(value));

        return new CustomerId(id);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public bool Equals(CustomerId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(CustomerId left, CustomerId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CustomerId left, CustomerId right)
    {
        return !(left == right);
    }
} 