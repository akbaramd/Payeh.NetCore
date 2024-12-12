using MediatR;
using Microsoft.EntityFrameworkCore;
using Payeh.SharedKernel.Domain;

namespace Payeh.SharedKernel.EntityFrameworkCore;

public class PayehDbContext<TDbContext> : DbContext where TDbContext : DbContext
{
    private readonly IMediator _mediator;

    public PayehDbContext(DbContextOptions<TDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public override int SaveChanges()
    {
        var result = base.SaveChanges();
        PublishDomainEventsAsync().Wait();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync();
        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        // Collect all entities that have domain events
        var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        // Collect all domain events
        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Publish domain events
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }

        // Clear domain events
        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }
    }
}