using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions; // Include Dynamic LINQ library

namespace Payeh.FluentQuery
{
    public static partial  class FluentQueryExtensions
    {
        // Apply dynamic filters to the query
        // Get the LINQ operator from the filter's operator
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, List<FilterItem> filters)
        {
            if (filters == null || !filters.Any())
                return query;

            foreach (var filter in filters)
            {
                query = ApplyFilter(query, filter);
            }

            return query;
        }

        private static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, FilterItem filter)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, filter.Field);
            // Get the property type
            var propertyType = property.Type;
            var filterValue = Convert.ChangeType(filter.Value, propertyType);   
            Expression body;

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
                    body = Expression.GreaterThan(property, Expression.Constant(Convert.ChangeType(filter.Value, property.Type)));
                    break;
                case "lt":
                    body = Expression.LessThan(property, Expression.Constant(Convert.ChangeType(filter.Value, property.Type)));
                    break;
                case "ge":
                    body = Expression.GreaterThanOrEqual(property, Expression.Constant(Convert.ChangeType(filter.Value, property.Type)));
                    break;
                case "le":
                    body = Expression.LessThanOrEqual(property, Expression.Constant(Convert.ChangeType(filter.Value, property.Type)));
                    break;
                default:
                    throw new NotImplementedException($"Operator '{filter.Operator}' is not supported");
            }

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(predicate);
        }

        // Apply dynamic sorting to the query
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, List<SortItem> sorts)
        {
            if (sorts == null || !sorts.Any())
                return query;

            var sortExpressions = new List<string>();

            foreach (var sort in sorts)
            {
                string sortExpression = $"{sort.Field} {sort.Direction}";
                sortExpressions.Add(sortExpression);
            }

            // Combine all sorting expressions into one string
            string combinedSort = string.Join(", ", sortExpressions);

            // Apply dynamic sorting using Dynamic LINQ
            return query.OrderBy(combinedSort);
        }

        // Apply pagination to the query
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int take, int skip)
        {
            if (skip > 0)
            {
                query = query.Skip(skip);
            }
            
            if (take > 0)
            {
                query = query.Take(take);
            }
            return query;
        }

        // Combine filters, sorting, and paging in a single method
        public static IQueryable<T> ApplyFluentQuery<T>(this IQueryable<T> query, FluentFilterQuery filterQuery)
        {
            // Apply filters, sorting, and pagination
            query = query.ApplyFilters(filterQuery.Filters);
            query = query.ApplySorting(filterQuery.Sorts);
            query = query.ApplyPaging(filterQuery.Take, filterQuery.Skip);

            return query;
        }
    }
}
