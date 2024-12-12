using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Payeh.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Represents a base class for value objects in a domain-driven design context.
/// Enforces immutability and provides equality operations.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>, IComparable<ValueObject>
{
    /// <summary>
    /// Determines whether the specified value object is equal to the current value object.
    /// </summary>
    /// <param name="other">The value object to compare with the current value object.</param>
    /// <returns>True if the specified value object is equal to the current value object; otherwise, false.</returns>
    public bool Equals(ValueObject? other)
    {
        // Check if the other object is null or of a different type
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        // Compare equality components for sequence equality
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value object.</param>
    /// <returns>True if the specified object is equal to the current value object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        // Ensure the object is of the same type and not null
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        return Equals(obj as ValueObject);
    }

    /// <summary>
    /// Serves as the hash function for the value object.
    /// </summary>
    /// <returns>A hash code for the current value object.</returns>
    public override int GetHashCode()
    {
        // Aggregate hash codes for all equality components
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked // Allow overflow to wrap around
                {
                    return (current * 23) + (obj?.GetHashCode() ?? 0);
                }
            });
    }

    /// <summary>
    /// Compares the current value object with another value object of the same type.
    /// </summary>
    /// <param name="other">The value object to compare to.</param>
    /// <returns>A signed integer that indicates the relative order of the objects being compared.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the other value object is null.</exception>
    public int CompareTo(ValueObject? other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        // Compare equality components sequentially
        using var thisEnumerator = GetEqualityComponents().GetEnumerator();
        using var otherEnumerator = other.GetEqualityComponents().GetEnumerator();

        while (thisEnumerator.MoveNext() && otherEnumerator.MoveNext())
        {
            var comparison = Comparer<object>.Default.Compare(thisEnumerator.Current, otherEnumerator.Current);

            if (comparison != 0)
            {
                return comparison; // Return the result of the first non-equal comparison
            }
        }

        return 0; // All components are equal
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>True if the value objects are equal; otherwise, false.</returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        // Handle both null and individual null cases
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>True if the value objects are not equal; otherwise, false.</returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns a string that represents the current value object, displaying its properties.
    /// </summary>
    /// <returns>A string representation of the value object.</returns>
    public override string ToString()
    {
        // Construct a string using property names and values
        var type = GetType();
        var properties = type.GetProperties();
        var values = properties.Select(p => $"{p.Name}: {p.GetValue(this) ?? "null"}");
        return $"{type.Name} ({string.Join(", ", values)})";
    }

    /// <summary>
    /// Creates a shallow copy of the current value object.
    /// </summary>
    /// <returns>A shallow copy of the current value object.</returns>
    public ValueObject Clone()
    {
        return (ValueObject)MemberwiseClone();
    }

    /// <summary>
    /// Provides a custom value comparer for Entity Framework Core when mapping value objects with collections.
    /// </summary>
    /// <typeparam name="T">The value object type.</typeparam>
    /// <returns>A value comparer for the specified value object type.</returns>
    public static ValueComparer<T> GetValueComparer<T>() where T : ValueObject
    {
        // Define equality, hash code, and cloning logic for EF Core
        return new ValueComparer<T>(
            (l, r) => l == r,
            v => v.GetHashCode(),
            v => (T)v.Clone()
        );
    }

    /// <summary>
    /// Converts the current value object into a JSON string representation.
    /// </summary>
    /// <returns>A JSON string representation of the value object.</returns>
    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// Parses a JSON string into a value object of the specified type.
    /// </summary>
    /// <typeparam name="T">The value object type.</typeparam>
    /// <param name="json">The JSON string representation of the value object.</param>
    /// <returns>The parsed value object.</returns>
    public static T FromJson<T>(string json) where T : ValueObject
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Invalid JSON string.");
    }

    /// <summary>
    /// Gets the components of the value object that are used for equality comparison.
    /// Must be overridden in derived classes to specify the properties that define equality.
    /// </summary>
    /// <returns>An enumeration of the components that define the value object's equality.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();
}
