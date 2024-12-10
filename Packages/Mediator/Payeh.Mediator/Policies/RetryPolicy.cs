using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payeh.Mediator.Abstractions.Policies;

namespace Payeh.Mediator.Policies
{
    /// <summary>
    /// Implements a retry mechanism that retries executing an action upon failure.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class RetryPolicy<TResponse> : IPolicy<TResponse>
    {
        private readonly int _maxRetryAttempts;
        private readonly TimeSpan _pauseBetweenFailures;
        private readonly ILogger<RetryPolicy<TResponse>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicy{TResponse}"/> class.
        /// </summary>
        /// <param name="maxRetryAttempts">The maximum number of retry attempts.</param>
        /// <param name="pauseBetweenFailures">The pause duration between retries.</param>
        /// <param name="logger">The logger instance.</param>
        public RetryPolicy(int maxRetryAttempts, TimeSpan pauseBetweenFailures, ILogger<RetryPolicy<TResponse>> logger)
        {
            if (maxRetryAttempts < 1)
                throw new ArgumentOutOfRangeException(nameof(maxRetryAttempts), "Retry attempts must be at least 1.");
            
            _maxRetryAttempts = maxRetryAttempts;
            _pauseBetweenFailures = pauseBetweenFailures;
            _logger = logger;
        }

        /// <summary>
        /// Executes the provided action with retry logic.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the action.</returns>
        public async Task<TResponse> ExecuteAsync(Func<Task<TResponse>> action)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    _logger.LogInformation($"RetryPolicy: Attempt {attempt} of {_maxRetryAttempts}.");
                    return await action();
                }
                catch (Exception ex) when (attempt < _maxRetryAttempts)
                {
                    _logger.LogWarning(ex, $"RetryPolicy: Attempt {attempt} failed. Retrying in {_pauseBetweenFailures.TotalSeconds} seconds...");
                    await Task.Delay(_pauseBetweenFailures);
                }
                catch
                {
                    // Rethrow the last exception if max attempts reached
                    throw;
                }
            }
        }
    }
}
