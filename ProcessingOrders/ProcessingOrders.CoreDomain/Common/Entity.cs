namespace ProcessingOrders.CoreDomain.Common;

public abstract class Entity<TId>
{
    int? _requestedHashCode;
    
    TId? _id;
    public TId Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }
    
    public bool IsTransient() => Id.Equals(default(TId));
    
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId>)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        var item = (Entity<TId>) obj;

        if (item.IsTransient() || IsTransient())
        {
            return false;
        }
        else
        {
            return item.Id.Equals(Id);
        }
    }
    
    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            _requestedHashCode ??= Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }
        else
            return base.GetHashCode();

    }
    
    public static bool operator == (Entity<TId> left, Entity<TId> right)
    {
        if (Equals(left, null))
        {
            return (Equals(right, null)) ? true : false;
        }
        else
        {
            return left.Equals(right);
        }
    }
    
    public static bool operator != (Entity<TId> left, Entity<TId> right)
    {
        return !(left == right);
    }
} 