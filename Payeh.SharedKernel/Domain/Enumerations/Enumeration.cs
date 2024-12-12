using System.Reflection;
using Payeh.SharedKernel.Domain.ValueObjects;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Payeh.SharedKernel.Exceptions;

namespace Payeh.SharedKernel.Domain.Enumerations;

/// <summary>
///     Base class for implementing strongly-typed enums with additional utility methods.
/// </summary>
[JsonConverter(typeof(EnumerationJsonConverter))]
public abstract class Enumeration : ValueObject, IComparable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Enumeration" /> class.
    /// </summary>
    /// <param name="id">The identifier of the enumeration.</param>
    /// <param name="name">The name of the enumeration.</param>
    protected Enumeration(int id, string name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new PayehException("Name cannot be null or whitespace.") : name;
        Id = id;
    }

    /// <summary>
    ///     Gets the name of the enumeration.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the identifier of the enumeration.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Compares this enumeration instance to another based on Id.
    /// </summary>
    /// <param name="other">The other enumeration instance to compare to.</param>
    /// <returns>An integer that indicates the relative order of the objects being compared.</returns>
    /// <exception cref="PayehException">Thrown when the other object is not of type <see cref="Enumeration" />.</exception>
    public int CompareTo(object? other)
    {
        if (other is not Enumeration otherEnumeration)
            throw new PayehException($"Object must be of type {nameof(Enumeration)}");

        return Id.CompareTo(otherEnumeration.Id);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Id;
    }

    /// <summary>
    ///     Returns a string representation of the enumeration.
    /// </summary>
    /// <returns>The name of the enumeration.</returns>
    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current enumeration.
    /// </summary>
    /// <param name="obj">The object to compare with the current enumeration.</param>
    /// <returns>True if the specified object is equal to the current enumeration; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Enumeration otherValue && GetType() == obj.GetType() && Id == otherValue.Id;
    }

    /// <summary>
    ///     Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    ///     Implicitly converts an enumeration to its integer Id.
    /// </summary>
    /// <param name="enumeration">The enumeration to convert.</param>
    /// <returns>The Id of the enumeration.</returns>
    public static implicit operator int(Enumeration enumeration)
    {
        return enumeration.Id;
    }

    /// <summary>
    ///     Implicitly converts an enumeration to its string Name.
    /// </summary>
    /// <param name="enumeration">The enumeration to convert.</param>
    /// <returns>The Name of the enumeration.</returns>
    public static implicit operator string(Enumeration enumeration)
    {
        return enumeration.Name;
    }

    /// <summary>
    ///     Retrieves all instances of the enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>An enumerable collection of all enumeration instances.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    /// <summary>
    ///     Retrieves an enumeration instance by its Id.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="id">The Id of the enumeration.</param>
    /// <returns>The enumeration instance if found; otherwise, null.</returns>
    public static T? FromId<T>(int id) where T : Enumeration
    {
        return GetAll<T>().FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    ///     Retrieves an enumeration instance by its Name.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="name">The Name of the enumeration.</param>
    /// <returns>The enumeration instance if found; otherwise, null.</returns>
    public static T? FromName<T>(string name) where T : Enumeration
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new PayehException("Name cannot be null or whitespace.");

        return GetAll<T>().FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     Tries to retrieve an enumeration instance by its Id.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="id">The Id of the enumeration.</param>
    /// <param name="result">The output enumeration instance if found; otherwise, null.</param>
    /// <returns>True if the enumeration instance is found; otherwise, false.</returns>
    public static bool TryParse<T>(int id, out T? result) where T : Enumeration
    {
        result = FromId<T>(id);
        return result != null;
    }

    /// <summary>
    ///     Tries to retrieve an enumeration instance by its Name.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="name">The Name of the enumeration.</param>
    /// <param name="result">The output enumeration instance if found; otherwise, null.</param>
    /// <returns>True if the enumeration instance is found; otherwise, false.</returns>
    public static bool TryParse<T>(string name, out T? result) where T : Enumeration
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new PayehException("Name cannot be null or whitespace.");

        result = FromName<T>(name);
        return result != null;
    }

    /// <summary>
    ///     Equality operator for Enumeration.
    /// </summary>
    /// <param name="left">The left Enumeration to compare.</param>
    /// <param name="right">The right Enumeration to compare.</param>
    /// <returns>True if both enumerations are equal; otherwise, false.</returns>
    public static bool operator ==(Enumeration? left, Enumeration? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    ///     Inequality operator for Enumeration.
    /// </summary>
    /// <param name="left">The left Enumeration to compare.</param>
    /// <param name="right">The right Enumeration to compare.</param>
    /// <returns>True if both enumerations are not equal; otherwise, false.</returns>
    public static bool operator !=(Enumeration? left, Enumeration? right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     ASP.NET Core Model Binder for Enumeration types.
    /// </summary>
    public class EnumerationModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;

            var enumType = bindingContext.ModelType;

            if (!enumType.IsAssignableTo(typeof(Enumeration)))
                throw new InvalidOperationException($"Model type must inherit from {nameof(Enumeration)}.");

            var method = typeof(Enumeration).GetMethod(nameof(FromName))?.MakeGenericMethod(enumType);

            if (method == null)
                throw new InvalidOperationException($"Could not find {nameof(FromName)} method for type {enumType}.");

            var result = method.Invoke(null, new object[] { value });

            bindingContext.Result = result != null
                ? ModelBindingResult.Success(result)
                : ModelBindingResult.Failed();

            return Task.CompletedTask;
        }
    }
}