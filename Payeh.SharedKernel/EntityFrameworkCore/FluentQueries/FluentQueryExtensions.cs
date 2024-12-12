using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Payeh.SharedKernel.EntityFrameworkCore.FluentQueries;

/// <summary>
/// Provides advanced filtering, sorting, pagination, and include capabilities for EF Core queries.
/// </summary>
public static class FluentQueryExtensions
{
    /// <summary>
    /// Applies filters to the query.
    /// </summary>
    public static IQueryable<T> ApplyFluentQueryFilters<T>(this IQueryable<T> query, List<FluentQueryFilter> filters)
    {
        // If no filters are provided, return the original query
        if (filters == null || !filters.Any())
            return query;

        // Apply each filter in the list
        foreach (var filter in filters)
        {
            query = ApplyFluentQueryFilter(query, filter);
        }

        return query;
    }

    private static IQueryable<T> ApplyFluentQueryFilter<T>(IQueryable<T> query, FluentQueryFilter filter)
    {
        // Create a parameter for the entity type
        var parameter = Expression.Parameter(typeof(T), "x");
        // Access the specified property on the entity
        var property = Expression.Property(parameter, filter.Field);
        // Determine the type of the property and convert the filter value to match
        var propertyType = property.Type;
        var filterValue = Convert.ChangeType(filter.Value, propertyType);
        Expression body;

        // Build the expression based on the specified operator
        switch (filter.Operator.ToLower())
        {
            case "equals":
                body = Expression.Equal(property, Expression.Constant(filterValue));
                break;
            case "contains":
                body = Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(filter.Value));
                break;
            case "startswith":
                body = Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(filter.Value));
                break;
            case "endswith":
                body = Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(filter.Value));
                break;
            case "gt":
                body = Expression.GreaterThan(property, Expression.Constant(filterValue));
                break;
            case "lt":
                body = Expression.LessThan(property, Expression.Constant(filterValue));
                break;
            case "ge":
                body = Expression.GreaterThanOrEqual(property, Expression.Constant(filterValue));
                break;
            case "le":
                body = Expression.LessThanOrEqual(property, Expression.Constant(filterValue));
                break;
            default:
                throw new NotImplementedException($"Operator '{filter.Operator}' is not supported");
        }

        // Build the predicate as a lambda expression
        var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
        // Apply the predicate to the query
        return query.Where(predicate);
    }

    /// <summary>
    /// Applies sorting to the query.
    /// </summary>
    public static IQueryable<T> ApplyFluentQuerySorting<T>(this IQueryable<T> query, List<FluentQuerySort> sorts)
    {
        // If no sorting is provided, return the original query
        if (sorts == null || !sorts.Any())
            return query;

        // Build sorting expressions
        var sortExpressions = sorts.Select(sort => $"{sort.Field} {sort.Direction}").ToList();
        string combinedSort = string.Join(", ", sortExpressions);
        // Apply sorting using Dynamic LINQ
        return query.OrderBy(combinedSort);
    }

    /// <summary>
    /// Applies pagination to the query.
    /// </summary>
    public static IQueryable<T> ApplyFluentQueryPaging<T>(this IQueryable<T> query, int take, int skip)
    {
        // Apply skip if specified
        if (skip > 0)
        {
            query = query.Skip(skip);
        }

        // Apply take if specified
        if (take > 0)
        {
            query = query.Take(take);
        }

        return query;
    }

    /// <summary>
    /// Applies include expressions to the query for eager loading.
    /// </summary>
    public static IQueryable<T> ApplyFluentQueryIncludes<T>(this IQueryable<T> query, string[] includes) where T : class
    {
        // If no includes are specified, return the original query
        if (includes == null || includes.Length == 0)
            return query;

        // Apply each include dynamically
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    /// <summary>
    /// Combines filtering, sorting, pagination, and includes into a single fluent query.
    /// </summary>
    public static IQueryable<T> ApplyFluentQuery<T>(this IQueryable<T> query, FluentQuery fluentQuery)where T : class
    {
        // Apply filters, sorting, pagination, and includes in sequence
        query = query.ApplyFluentQueryFilters(fluentQuery.Filters);
        query = query.ApplyFluentQuerySorting(fluentQuery.Sorts);
        query = query.ApplyFluentQueryPaging(fluentQuery.Take, fluentQuery.Skip);
        query = query.ApplyFluentQueryIncludes(fluentQuery.Includes);
        return query;
    }
}

