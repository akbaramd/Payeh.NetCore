using Payeh.SharedKernel.Domain.DomainEvents;
using System.Text.Json.Serialization;

namespace Payeh.SharedKernel.Domain;

/// <summary>
/// Represents the base class for aggregate roots in the domain.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the list of domain events associated with this aggregate.
    /// This property exposes domain events as a read-only collection to prevent external modification.
    /// </summary>
    [JsonIgnore] // Ensures that domain events are not serialized.
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Registers a domain event for this aggregate.
    /// Adding a domain event means it will be processed later by the event dispatcher.
    /// </summary>
    /// <param name="domainEvent">The domain event to register.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a specific domain event from the aggregate.
    /// This can be used if the event should no longer be processed.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events for this aggregate.
    /// Typically called after the events have been processed.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Applies a given domain event to the aggregate.
    /// This method both registers the event and applies any associated business logic.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply.</param>
    protected void ApplyDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

        AddDomainEvent(domainEvent);
        // Additional logic for applying the event can be added here.
    }
}

