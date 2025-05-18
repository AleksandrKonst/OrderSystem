namespace ProcessingOrders.CoreDomain.Common;

public abstract class Entity<TId>
{
    public required TId Id { get; set; }

    private bool IsTransient() => Id != null && Id.Equals(default(TId));
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Entity<TId>)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }

    private bool Equals(Entity<TId> obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        if (obj.IsTransient() || IsTransient())
        {
            return false;
        }
        return obj.Id != null && obj.Id.Equals(Id);
    }
    
    public static bool operator == (Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, null) ? Equals(right, null) : left.Equals(right);
    }
    
    public static bool operator != (Entity<TId> left, Entity<TId> right)
    {
        return !(left == right);
    }
} 