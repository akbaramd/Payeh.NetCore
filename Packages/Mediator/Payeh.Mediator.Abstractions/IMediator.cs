using System.Threading;
using System.Threading.Tasks;
using Payeh.Mediator.Abstractions.Events;
using Payeh.Mediator.Abstractions.Requests;

namespace Payeh.Mediator.Abstractions
{
    /// <summary>
    /// The mediator interface that sends requests to their associated handlers.
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Sends a request through the pipeline to the appropriate handler and returns a response.
        /// </summary>
        /// <typeparam name="TResponse">The response type of the request.</typeparam>
        /// <param name="request">The request instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Publishes an event (IEvent) to all registered event handlers.
        /// </summary>
        /// <param name="event">The event instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task Publish(IEvent @event, CancellationToken cancellationToken = default);
    }
}