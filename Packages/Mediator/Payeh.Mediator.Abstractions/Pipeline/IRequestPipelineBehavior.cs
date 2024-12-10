using System;
using System.Threading;
using System.Threading.Tasks;
using Payeh.Mediator.Abstractions.Events;
using Payeh.Mediator.Abstractions.Requests;

namespace Payeh.Mediator.Abstractions.Pipeline
{
    /// <summary>
    /// Represents a pipeline behavior that can run before and/or after the request handler is invoked.
    /// Behaviors can be chained to add cross-cutting concerns such as logging, validation, caching, etc.
    /// </summary>
    /// <typeparam name="TRequest">The request type implementing <see cref="IRequest{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public interface IRequestPipelineBehavior<TRequest, TResponse>: IPipelineBehavior where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Handles the request before and/or after the request handler is called.
        /// The <paramref name="next"/> function can be invoked to proceed to the next behavior or the handler.
        /// </summary>
        /// <param name="request">The request instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="next">The next function in the behavior chain or the request handler itself.</param>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task<TResponse>> next);
    }
    
    public interface IEventPipelineBehavior<TEvent>: IPipelineBehavior where TEvent : IEvent 
    {
        /// <summary>
        /// Handles the request before and/or after the request handler is called.
        /// The <paramref name="next"/> function can be invoked to proceed to the next behavior or the handler.
        /// </summary>
        /// <param name="request">The request instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="next">The next function in the behavior chain or the request handler itself.</param>
        Task Handle(TEvent request, CancellationToken cancellationToken, Func<TEvent, CancellationToken,Task> next);
    }
    
    public interface IPipelineBehavior
    {
       
    }
}