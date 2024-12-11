using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Payeh.FluentQuery
{
    public static partial class EntityFrameworkCoreFluentQueryExtensions
    {
        // Apply dynamic includes to the query
        public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> query, string[] includes) where T:class
        {
            if (includes == null || includes.Length == 0)
                return query;

            foreach (var include in includes)
            {
                query = query.Include(include); // Dynamically apply the Include
            }

            return query;
        }

        // Combine filters, sorting, paging, and includes in a single method
        public static IQueryable<T> ApplyFluentQuery<T>(
            this IQueryable<T> query,
            EntityFrameworkFluentFilterQuery filterQuery) where T : class
        {
            if (filterQuery == null)
                throw new ArgumentNullException(nameof(filterQuery));

            // Apply filters
            query = query.ApplyFilters(filterQuery.Filters);

            // Apply sorting
            query = query.ApplySorting(filterQuery.Sorts);

            // Apply pagination
            query = query.ApplyPaging(filterQuery.Take, filterQuery.Skip);

            // Apply includes
            query = query.ApplyIncludes(filterQuery.Includes);

            return query;
        }
    }
}