using Microsoft.Extensions.Logging;
using Payeh.Mediator.Abstractions.Policies;

namespace Payeh.Mediator.Policies
{
    /// <summary>
    /// Factory responsible for creating instances of policies.
    /// </summary>
    public class PolicyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
        public PolicyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a retry policy with the specified configuration.
        /// </summary>
        /// <param name="maxRetryAttempts">The maximum number of retry attempts.</param>
        /// <param name="pauseBetweenFailures">The pause duration between retries.</param>
        /// <returns>An instance of <see cref="RetryPolicy"/>.</returns>
        public IPolicy<TResponse> CreateRetryPolicy<TResponse>(int maxRetryAttempts, TimeSpan pauseBetweenFailures)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<RetryPolicy<TResponse>>>();
            return new RetryPolicy<TResponse>(maxRetryAttempts, pauseBetweenFailures, logger);
        }

        /// <summary>
        /// Creates a fallback policy with the specified fallback action.
        /// </summary>
        /// <typeparam name="TResponse">The type of the fallback response.</typeparam>
        /// <param name="fallbackAction">The fallback action to execute when the main action fails.</param>
        /// <returns>An instance of <see cref="FallbackPolicy{TResponse}"/>.</returns>
        public IPolicy<TResponse> CreateFallbackPolicy<TResponse>(Func<TResponse> fallbackAction)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<FallbackPolicy<TResponse>>>();
            return new FallbackPolicy<TResponse>(fallbackAction, logger);
        }
    }
}
