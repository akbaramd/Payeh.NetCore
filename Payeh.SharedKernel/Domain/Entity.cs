namespace Payeh.SharedKernel.Domain;

/// <summary>
/// Base class for all entities in the domain, providing a method to retrieve the entity's keys.
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    protected Entity()
    {
    }

    /// <summary>
    /// Gets the array of keys that uniquely identify the entity.
    /// </summary>
    /// <returns>An object representing the entity's key(s).</returns>
    public abstract object GetKey();

    /// <summary>
    /// Determines whether the current entity is equal to another entity.
    /// </summary>
    /// <param name="other">The other entity to compare with.</param>
    /// <returns>True if the entities are equal; otherwise, false.</returns>
    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        // Compare entities by their type and key(s)
        return GetType() == other.GetType() &&
               Equals(GetKey(), other.GetKey());
    }

    public override bool Equals(object? obj)
    {
        if (obj is Entity other)
            return Equals(other);

        return false;
    }

    public override int GetHashCode()
    {
        // Use the hash code of the key(s) for consistent hashing
        var key = GetKey();
        return key != null ? key.GetHashCode() : 0;
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Provides a string representation of the entity's key(s) for debugging.
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Key={GetKey()}]";
    }
}

