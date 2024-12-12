using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payeh.SharedKernel.Domain;
using Payeh.SharedKernel.Domain.ValueObjects;

namespace Payeh.SharedKernel.EntityFrameworkCore.Domain;

/// <summary>
/// Provides extension methods to configure Entity Framework models for ValueObjects, BusinessIds, and AggregateRoots.
/// </summary>
public static class DomainModelBuilderExtensions
{
    /// <summary>
    /// Configures all models to support ValueObjects, BusinessIds, and AggregateRoots.
    /// </summary>
    /// <param name="modelBuilder">The EF Core ModelBuilder instance.</param>
    public static void ConfigureDomain(this ModelBuilder modelBuilder)
    {
        var types = modelBuilder.Model.GetEntityTypes();

       
        Console.WriteLine("Configuring models for ValueObjects, BusinessIds, and AggregateRoots...");
        modelBuilder.ConfigureValueObjects();
        modelBuilder.ConfigureBusinessIds();
        modelBuilder.ConfigureAggregateRoots();
        Console.WriteLine("Model configuration completed.");
    }

    /// <summary>
    /// Configures all ValueObject properties for the given model, including nested ValueObjects.
    /// </summary>
    /// <param name="modelBuilder">The EF Core ModelBuilder instance.</param>
    public static void ConfigureValueObjects(this ModelBuilder modelBuilder)
    {
        Console.WriteLine("Configuring ValueObjects...");
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureValueObjectProperties(modelBuilder, entityType.ClrType);
        }
        Console.WriteLine("ValueObject configuration completed.");
    }

    private static void ConfigureValueObjectProperties(ModelBuilder modelBuilder, Type entityType)
    {
        foreach (var property in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (typeof(ValueObject).IsAssignableFrom(property.PropertyType))
            {
                Console.WriteLine($"Configuring ValueObject property '{property.Name}' on entity '{entityType.Name}'...");
                modelBuilder.Entity(entityType)
                    .OwnsOne(property.PropertyType, property.Name, navigationBuilder =>
                    {
                        Console.WriteLine($"Mapping ValueObject property '{property.Name}'...");
                        ConfigureValueObject(property.Name, navigationBuilder);
                    });
            }
            else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                Console.WriteLine($"Checking nested properties for potential ValueObjects in '{property.Name}'...");
                ConfigureValueObjectProperties(modelBuilder, property.PropertyType);
            }
        }
    }

    /// <summary>
    /// Configures all BusinessId properties for the given model.
    /// </summary>
    /// <param name="modelBuilder">The EF Core ModelBuilder instance.</param>
    public static void ConfigureBusinessIds(this ModelBuilder modelBuilder)
    {
        Console.WriteLine("Configuring BusinessIds...");
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType.GetProperties())
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(BusinessId<,>))
                {
                    Console.WriteLine($"Configuring BusinessId property '{property.Name}' on entity '{entityType.ClrType.Name}'...");
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasColumnName(property.Name + "Id");
                }
            }
        }
        Console.WriteLine("BusinessId configuration completed.");
    }

    /// <summary>
    /// Configures AggregateRoot entities to ensure DomainEvents are not persisted.
    /// </summary>
    /// <param name="modelBuilder">The EF Core ModelBuilder instance.</param>
    public static void ConfigureAggregateRoots(this ModelBuilder modelBuilder)
    {
        Console.WriteLine("Configuring AggregateRoots...");
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AggregateRoot).IsAssignableFrom(entityType.ClrType))
            {
                Console.WriteLine($"Configuring AggregateRoot '{entityType.ClrType.Name}'...");
                modelBuilder.Entity(entityType.ClrType)
                    .Ignore("DomainEvents");
            }
        }
        Console.WriteLine("AggregateRoot configuration completed.");
    }

    /// <summary>
    /// Configures a ValueObject property to use a single table column for serialization.
    /// </summary>
    /// <param name="propertyName">The name of the property being configured.</param>
    /// <param name="navigationBuilder">The builder for the ValueObject navigation.</param>
    private static void ConfigureValueObject(string propertyName, OwnedNavigationBuilder navigationBuilder)
    {
        Console.WriteLine($"Configuring column mapping for ValueObject property '{propertyName}'...");
        navigationBuilder.Property("Value")
            .HasColumnName(propertyName + "Value");
        Console.WriteLine($"Column mapping completed for ValueObject property '{propertyName}'.");
    }
}
