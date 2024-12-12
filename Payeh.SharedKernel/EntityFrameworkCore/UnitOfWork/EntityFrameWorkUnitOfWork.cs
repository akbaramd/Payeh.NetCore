using Microsoft.EntityFrameworkCore;
using Payeh.SharedKernel.UnitOfWork;



namespace Payeh.SharedKernel.EntityFrameworkCore.UnitOfWork;

using MediatR;
using Payeh.SharedKernel.Domain;
using Payeh.SharedKernel.Domain.Repositories;
using Payeh.SharedKernel.EntityFrameworkCore.Domain;

/// <summary>
/// Implements an advanced Unit of Work with support for child units of work and reservations.
/// </summary>
public class EntityFrameworkUnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
{
    private readonly TDbContext _context;
    private readonly IMediator _mediator;
    private readonly Dictionary<Type, object> _repositories = new();

    private readonly Stack<EntityFrameworkUnitOfWork<TDbContext>> _childUnitOfWorks = new();

    public EntityFrameworkUnitOfWork(TDbContext context, IMediator mediator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
    {
        if (!_repositories.ContainsKey(typeof(TEntity)))
        {
            var repositoryInstance = new EntityFrameworkRepository<TEntity, TDbContext>(_context);
            _repositories[typeof(TEntity)] = repositoryInstance;
        }

        return (IRepository<TEntity>)_repositories[typeof(TEntity)];
    }

    public async Task<int> CommitAsync()
    {
        await PublishDomainEventsAsync();
        return await _context.SaveChangesAsync();
    }

    public int Commit()
    {
        PublishDomainEventsAsync().Wait();
        return _context.SaveChanges();
    }

    private async Task PublishDomainEventsAsync()
    {
        var entitiesWithEvents = _context.ChangeTracker.Entries<AggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }
    }

    /// <summary>
    /// Creates a child unit of work for nested operations.
    /// </summary>
    public EntityFrameworkUnitOfWork<TDbContext> CreateChildUnitOfWork()
    {
        var childUnitOfWork = new EntityFrameworkUnitOfWork<TDbContext>(_context, _mediator);
        _childUnitOfWorks.Push(childUnitOfWork);
        return childUnitOfWork;
    }

    /// <summary>
    /// Applies reservations made during a child unit of work to the parent context.
    /// </summary>
    public void ApplyReservations()
    {
        while (_childUnitOfWorks.Count > 0)
        {
            var childUnitOfWork = _childUnitOfWorks.Pop();
            foreach (var entry in childUnitOfWork._context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    _context.Entry(entry.Entity).State = entry.State;
                }
            }
        }
    }

    public void Dispose()
    {
        while (_childUnitOfWorks.Count > 0)
        {
            _childUnitOfWorks.Pop().Dispose();
        }

        _context.Dispose();
    }
}
