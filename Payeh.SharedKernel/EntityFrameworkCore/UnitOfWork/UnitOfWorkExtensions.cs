using Microsoft.EntityFrameworkCore;
using Payeh.SharedKernel.UnitOfWork;

namespace Payeh.SharedKernel.EntityFrameworkCore.UnitOfWork;

/// <summary>
/// Provides extension methods to register Unit of Work and repositories in the service container.
/// </summary>
public static class UnitOfWorkExtensions
{
    /// <summary>
    /// Registers the Unit of Work and repositories with the specified DbContext in the service container.
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        // Register the DbContext
        services.AddDbContext<TContext>();

        // Register the Unit of Work
        services.AddScoped<IUnitOfWork, EntityFrameworkUnitOfWork<TContext>>();

        return services;
    }
}