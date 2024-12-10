using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payeh.Mediator.Abstractions.Policies;

namespace Payeh.Mediator.Policies
{
    /// <summary>
    /// Implements a fallback mechanism that provides a default response when an action fails.
    /// </summary>
    /// <typeparam name="TResponse">The type of the fallback response.</typeparam>
    public class FallbackPolicy<TResponse> : IPolicy<TResponse>
    {
        private readonly Func<TResponse> _fallbackAction;
        private readonly ILogger<FallbackPolicy<TResponse>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackPolicy{TResponse}"/> class.
        /// </summary>
        /// <param name="fallbackAction">The fallback action to execute when the main action fails.</param>
        /// <param name="logger">The logger instance.</param>
        public FallbackPolicy(Func<TResponse> fallbackAction, ILogger<FallbackPolicy<TResponse>> logger)
        {
            _fallbackAction = fallbackAction ?? throw new ArgumentNullException(nameof(fallbackAction));
            _logger = logger;
        }

        /// <summary>
        /// Executes the fallback policy by returning the fallback action's result.
        /// </summary>
        /// <param name="action">The action to execute (ignored in fallback).</param>
        /// <returns>The fallback response.</returns>
        public Task<TResponse> ExecuteAsync(Func<Task<TResponse>> action)
        {
            _logger.LogInformation("FallbackPolicy: Executing fallback action.");
            return Task.FromResult(_fallbackAction());
        }
    }
}