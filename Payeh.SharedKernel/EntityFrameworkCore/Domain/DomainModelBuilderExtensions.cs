using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Payeh.SharedKernel.Domain;
using Payeh.SharedKernel.Domain.Enumerations;
using Payeh.SharedKernel.Domain.ValueObjects;
using System;
using System.Linq;
using Payeh.SharedKernel.EntityFrameworkCore.Domain;

namespace Microsoft.EntityFrameworkCore
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder DomainConfiguration(this EntityTypeBuilder b)
        {
            Console.WriteLine($"Configuring entity: {b.Metadata.ClrType.Name}");

            b.ConfigureBusinessIdProperties();
            b.ConfigureEnumerationProperties();
            b.IgnoreDomainEventProperties();

            Console.WriteLine($"Configuration completed for entity: {b.Metadata.ClrType.Name}");
            return b;
        }

        private static void IgnoreDomainEventProperties(this EntityTypeBuilder b)
        {
            var entityType = b.Metadata.ClrType;
            if (entityType.IsAssignableTo(typeof(AggregateRoot)))
            {
                Console.WriteLine($"Ignoring 'DomainEvents' property for AggregateRoot: {entityType.Name}");
                b.Ignore(nameof(AggregateRoot.DomainEvents));
            }
        }

        public static EntityTypeBuilder ConfigureEnumerationProperties(this EntityTypeBuilder builder)
        {
            var entityType = builder.Metadata.ClrType;
            Console.WriteLine($"Configuring Enumeration properties for entity: {entityType.Name}");

            var enumerationProperties = entityType
                .GetProperties()
                .Where(property => IsEnumerationType(property.PropertyType))
                .ToList();

            foreach (var property in enumerationProperties)
            {
                try
                {
                    Console.WriteLine($"Configuring Enumeration property: {property.Name}");

                    var propertyType = property.PropertyType;
                    var isNullable = IsNullableEnumerationType(propertyType);
                    var underlyingType = isNullable ? Nullable.GetUnderlyingType(propertyType) : propertyType;

                    if (underlyingType == null) continue;

                    builder.OwnsOne(propertyType, property.Name, c =>
                    {
                        c.Property<int>("Id") // Map the `Id` property
                            .HasColumnName($"{property.Name}Id")
                            .IsRequired(!isNullable);

                        c.Property<string>("Name") // Map the `Name` property
                            .HasColumnName($"{property.Name}Name")
                            .IsRequired(!isNullable);
                    });

                    Console.WriteLine($"Enumeration property '{property.Name}' configured successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configuring Enumeration property '{property.Name}': {ex.Message}");
                }
            }

            return builder;
        }

        public static EntityTypeBuilder ConfigureBusinessIdProperties(this EntityTypeBuilder builder)
        {
            var entityType = builder.Metadata.ClrType;
            Console.WriteLine($"Configuring BusinessId properties for entity: {entityType.Name}");

            var businessIdProperties = entityType
                .GetProperties()
                .Where(property => IsBusinessIdType(property.PropertyType))
                .ToList();

            foreach (var property in businessIdProperties)
            {
                try
                {
                    Console.WriteLine($"Configuring BusinessId property: {property.Name}");
                    var propertyType = property.PropertyType;

                    if (IsGenericBonBusinessIdWithKey(propertyType))
                    {
                        var keyType = GetKeyType(propertyType);

                        var converterType =
                            typeof(BusinessIdConverter<,>).MakeGenericType(propertyType, keyType);
                        if (Activator.CreateInstance(converterType) is ValueConverter converterInstance)
                        {
                            builder.Property(property.Name)
                                .HasConversion(converterInstance)
                                .HasColumnName(property.Name);
                        }
                    }
                    else if (IsGenericBonBusinessId(propertyType))
                    {
                        var converterType = typeof(BusinessIdConverter<>).MakeGenericType(propertyType);
                        if (Activator.CreateInstance(converterType) is ValueConverter converterInstance)
                        {
                            builder.Property(property.Name)
                                .HasConversion(converterInstance)
                                .HasColumnName(property.Name);
                        }
                    }

                    Console.WriteLine($"BusinessId property '{property.Name}' configured successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configuring BusinessId property '{property.Name}': {ex.Message}");
                }
            }

            return builder;
        }

        private static bool IsGenericBonBusinessIdWithKey(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BusinessId<,>))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private static bool IsGenericBonBusinessId(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BusinessId<>))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private static Type GetKeyType(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BusinessId<,>))
                {
                    return type.GetGenericArguments()[1];
                }

                type = type.BaseType;
            }

            throw new InvalidOperationException($"Type '{type.Name}' is not a valid BusinessId with a key.");
        }

        private static bool IsBusinessIdType(Type type)
        {
            if (type == null) return false;

            if (IsGenericBonBusinessId(type) || IsGenericBonBusinessIdWithKey(type))
                return true;

            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (IsGenericBonBusinessId(baseType) || IsGenericBonBusinessIdWithKey(baseType))
                    return true;

                baseType = baseType.BaseType;
            }

            return false;
        }

        private static bool IsEnumerationType(Type type)
        {
            return type != null && type.IsSubclassOf(typeof(Enumeration));
        }

        private static bool IsNullableEnumerationType(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   type.GetGenericArguments()[0].IsSubclassOf(typeof(Enumeration));
        }
    }
}
