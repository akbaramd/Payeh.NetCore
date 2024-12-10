using System.Threading;
using System.Threading.Tasks;
using Payeh.Mediator.Abstractions.Requests;

namespace Payeh.Mediator.Abstractions.Handlers
{
    /// <summary>
    /// Defines a handler for a given request type <typeparamref name="TRequest"/> that returns a response <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Handles a given request and returns a response.
        /// </summary>
        /// <param name="request">The request to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}