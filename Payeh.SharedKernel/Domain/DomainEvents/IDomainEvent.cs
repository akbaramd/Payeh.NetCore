using MediatR;

namespace Payeh.SharedKernel.Domain.DomainEvents;

/// <summary>
/// Represents a domain event in the application, implementing the MediatR INotification interface.
/// </summary>
public interface IDomainEvent : INotification
{
}

