using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payeh.FluentQuery;
using Xunit;

public class FluentQueryExtensionsTests
{
    private TestDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("Filename=:memory:") // Use SQLite in-memory database
            .Options;

        var context = new TestDbContext(options);
        context.Database.OpenConnection(); // Open the database connection
        context.Database.EnsureCreated(); // Ensure schema is created
        context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

        // Seed Categories
        if (!context.Categories.Any())
        {
            var parentCategory = new Category { Id = 3, Name = "ParentCategory" };
            context.Categories.Add(parentCategory);

            var electronicsCategory = new Category { Id = 1, Name = "Electronics", ParentCategory = parentCategory };
            var furnitureCategory = new Category { Id = 2, Name = "Furniture" };

            context.Categories.AddRange(electronicsCategory, furnitureCategory);
            context.SaveChanges(); // Save Categories first
        }

        // Seed Products
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Id = 1, Name = "Laptop", CategoryId = 1 }, // Valid CategoryId
                new Product { Id = 2, Name = "Chair", CategoryId = 2 },  // Valid CategoryId
                new Product { Id = 3, Name = "Smartphone", CategoryId = 1 } // Valid CategoryId
            );
            context.SaveChanges(); // Save Products
        }

        return context;
    }


    [Fact]
    public void Test_ApplyFluentQuery_FiltersSortingPaging()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Take = 2,
            Skip = 0,
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Name", Operator = "contains", Value = "h" }
            },
            Sorts = new List<SortItem>
            {
                new SortItem { Field = "Id", Direction = "asc" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Equal(2, result.Count); // Only 2 items due to Take
        Assert.Contains(result, p => p.Name == "Chair"); // Chair matches filter
        Assert.DoesNotContain(result, p => p.Name == "Laptop"); // Smartphone skipped
    }

    [Fact]
    public void Test_ApplyEntityFrameworkFluentQuery_WithIncludes()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new EntityFrameworkFluentFilterQuery
        {
            Take = 10,
            Skip = 0,
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "CategoryId", Operator = "equals", Value = "1" }
            },
            Includes = new[] { "Category" }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Equal(2, result.Count); // 2 products belong to CategoryId = 1
        Assert.All(result, p => Assert.NotNull(p.Category)); // Ensure Category is included
        Assert.Contains(result, p => p.Name == "Laptop"); // Ensure Laptop is in results
    }

    [Fact]
    public void Test_Operator_Equals()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Name", Operator = "equals", Value = "Laptop" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Single(result); // Only one product matches
        Assert.Equal("Laptop", result.First().Name); // Verify the product name
    }

    [Fact]
    public void Test_Operator_Contains()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Name", Operator = "contains", Value = "Lap" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Single(result); // Only one product matches
        Assert.Equal("Laptop", result.First().Name); // Verify the product name
    }

    [Fact]
    public void Test_Operator_StartsWith()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Name", Operator = "startswith", Value = "Lap" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Single(result); // Only one product matches
        Assert.Equal("Laptop", result.First().Name); // Verify the product name
    }

    [Fact]
    public void Test_Operator_EndsWith()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Name", Operator = "endswith", Value = "phone" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Single(result); // Only one product matches
        Assert.Equal("Smartphone", result.First().Name); // Verify the product name
    }

    [Fact]
    public void Test_Operator_GreaterThan()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Id", Operator = "gt", Value = "1" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Equal(2, result.Count); // Two products have Id > 1
        Assert.Contains(result, p => p.Id == 2); // Verify one of the products
    }

    [Fact]
    public void Test_Operator_LessThan()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Id", Operator = "lt", Value = "2" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Single(result); // Only one product has Id < 2
        Assert.Equal(1, result.First().Id); // Verify the product ID
    }

    [Fact]
    public void Test_Operator_GreaterThanOrEqual()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Id", Operator = "ge", Value = "2" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Equal(2, result.Count); // Two products have Id >= 2
        Assert.Contains(result, p => p.Id == 2); // Verify one of the products
    }

    [Fact]
    public void Test_Operator_LessThanOrEqual()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();
        var filterQuery = new FluentFilterQuery
        {
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "Id", Operator = "le", Value = "2" }
            }
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Equal(2, result.Count); // Two products have Id <= 2
        Assert.Contains(result, p => p.Id == 1); // Verify one of the products
    }

    [Fact]
    public void Test_ApplyFluentQuery_WithIncludes()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();

        var filterQuery = new EntityFrameworkFluentFilterQuery
        {
            Take = 10,
            Skip = 0,
            Includes = new[] { "Category" } // Include the related Category entity
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Equal(3, result.Count); // There are 3 products in total
        Assert.All(result, product => Assert.NotNull(product.Category)); // Ensure Category is included
    }

    [Fact]
    public void Test_ApplyFluentQuery_WithIncludes_AndFilters()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();

        var filterQuery = new EntityFrameworkFluentFilterQuery
        {
            Take = 10,
            Skip = 0,
            Filters = new List<FilterItem>
            {
                new FilterItem { Field = "CategoryId", Operator = "equals", Value = "1" }
            },
            Includes = new[] { "Category" } // Include the related Category entity
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Equal(2, result.Count); // Only products in CategoryId = 1
        Assert.All(result, product => Assert.NotNull(product.Category)); // Ensure Category is included
        Assert.All(result, product => Assert.Equal(1, product.CategoryId)); // Verify the filter
    }

    [Fact]
    public void Test_ApplyFluentQuery_WithMultipleIncludes()
    {
        // Arrange
        var context = CreateDbContext();
        var query = context.Products.AsQueryable();

        // Add a new related entity for testing
        if (!context.Categories.Any(c => c.Name == "ParentCategory"))
        {
            var parentCategory = new Category { Id = 3, Name = "ParentCategory" };
            context.Categories.Add(parentCategory);
            context.Categories.First(c => c.Id == 1).ParentCategory = parentCategory;
            context.SaveChanges();
        }

        var filterQuery = new EntityFrameworkFluentFilterQuery
        {
            Take = 10,
            Skip = 0,
            Includes = new[] { "Category", "Category.ParentCategory" } // Include nested entities
        };

        // Act
        var result = query.ApplyFluentQuery(filterQuery).ToList();

        // Assert
        Assert.Equal(3, result.Count); // Ensure all products are returned
        Assert.All(result, product => Assert.NotNull(product.Category)); // Ensure Category is included
        Assert.All(result, product =>
        {
            if (product.CategoryId == 1) // Products with CategoryId = 1 should have a ParentCategory
            {
                Assert.NotNull(product.Category.ParentCategory);
            }
            else
            {
                Assert.Null(product.Category.ParentCategory); // ParentCategory can be null for others
            }
        });
    }

}