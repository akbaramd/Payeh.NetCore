using System.Threading;
using System.Threading.Tasks;

namespace Payeh.Mediator.Abstractions.Events
{
    /// <summary>
    /// Defines a handler for an event (IEvent). Multiple handlers can handle the same event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// Handles the given event.
        /// </summary>
        /// <param name="event">The event instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task Handle(TEvent @event, CancellationToken cancellationToken);
    }
}