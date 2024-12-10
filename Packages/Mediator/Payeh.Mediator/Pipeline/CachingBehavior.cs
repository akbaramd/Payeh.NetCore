using Microsoft.Extensions.Caching.Memory;
using Payeh.Mediator.Options;

namespace Payeh.Mediator.Pipeline
{
    /// <summary>
    /// Caches the response of a request to prevent redundant processing.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class CachingBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
        private readonly TimeSpan _cacheDuration;

        public CachingBehavior(MediatorConfigurator configurator,IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
            _cacheDuration = TimeSpan.FromSeconds(configurator.Caching.DefaultCacheDuration);
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task<TResponse>> next)
        {
            var cacheKey = $"{typeof(TRequest).FullName}:{request.GetHashCode()}";

            if (_cache.TryGetValue(cacheKey, out TResponse cachedResponse))
            {
                _logger.LogInformation($"Cache hit for request {typeof(TRequest).Name}");
                return cachedResponse;
            }

            _logger.LogInformation($"Cache miss for request {typeof(TRequest).Name}. Processing...");
            var response = await next(request, cancellationToken);

            _cache.Set(cacheKey, response, _cacheDuration);
            _logger.LogInformation($"Response cached for request {typeof(TRequest).Name} for {_cacheDuration.TotalMinutes} minutes.");

            return response;
        }
    }
}
