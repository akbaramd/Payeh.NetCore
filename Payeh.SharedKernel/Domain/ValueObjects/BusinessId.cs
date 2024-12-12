using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json.Serialization;

namespace Payeh.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Represents a base class for business identifiers, providing strong typing and immutability.
/// </summary>
[JsonConverter(typeof(BusinessIdJsonConverterFactory))]
public abstract class BusinessId<T, TKey> : ValueObject, IEquatable<BusinessId<T, TKey>>
    where T : BusinessId<T, TKey>, new()
{
    /// <summary>
    /// Protected constructor for derived classes.
    /// </summary>
    protected BusinessId()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessId{T, TKey}" /> class with a specific value.
    /// </summary>
    /// <param name="value">The value of the business ID.</param>
    protected BusinessId(TKey value)
    {
        if (value == null || value.Equals(default(TKey)))
            throw new ArgumentException($"The value of {nameof(value)} cannot be null or default.", nameof(value));

        Value = value;
    }

    public TKey Value { get; private set; }

    /// <summary>
    /// Determines whether this instance and another specified instance have the same value.
    /// </summary>
    public bool Equals(BusinessId<T, TKey>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return EqualityComparer<TKey>.Default.Equals(Value, other.Value);
    }

    /// <summary>
    /// Factory method to create an instance of the derived class from an existing value.
    /// </summary>
    public static T FromValue(TKey value)
    {
        if (value == null || value.Equals(default(TKey)))
            throw new ArgumentException($"The value of {nameof(value)} cannot be null or default.", nameof(value));

        return NewId(value);
    }

    /// <summary>
    /// Helper method to create an instance of the derived class.
    /// </summary>
    public static T NewId(TKey value)
    {
        var instance = new T();
        instance.Value = value;
        return instance;
    }

    public override bool Equals(object? obj)
    {
        if (obj is BusinessId<T, TKey> other)
            return Equals(other);

        return false;
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Overrides equality operators for value comparison.
    /// </summary>
    public static bool operator ==(BusinessId<T, TKey>? left, BusinessId<T, TKey>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(BusinessId<T, TKey>? left, BusinessId<T, TKey>? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Overrides equality components for comparing value objects.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Provides a custom value comparer for EF Core.
    /// </summary>
    public static ValueComparer<T> GetValueComparer()
    {
        return new ValueComparer<T>(
            (l, r) => l == r,
            v => v.GetHashCode(),
            v => (T)v.Clone()
        );
    }
}

/// <summary>
/// Represents a specialized implementation of BusinessId with a GUID as the key type.
/// </summary>
public abstract class BusinessId<T> : BusinessId<T, Guid> where T : BusinessId<T>, new()
{
    /// <summary>
    /// Initializes a new instance of the BusinessId class with a new GUID.
    /// </summary>
    public BusinessId() : base(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Initializes a new instance of the BusinessId class with a specific GUID value.
    /// </summary>
    public BusinessId(Guid value) : base(value)
    {
    }

    /// <summary>
    /// Factory method to create a new BusinessId with a new GUID.
    /// </summary>
    public static T NewId()
    {
        return FromValue(Guid.NewGuid());
    }

    /// <summary>
    /// Factory method to create a BusinessId from a string representation of a GUID.
    /// </summary>
    public static T FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Business ID cannot be empty or whitespace.", nameof(value));

        if (!Guid.TryParse(value, out var parsedGuid))
            throw new ArgumentException("Invalid GUID format.", nameof(value));

        return FromValue(parsedGuid);
    }
}