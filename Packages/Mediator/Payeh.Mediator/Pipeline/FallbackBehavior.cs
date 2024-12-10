using Payeh.Mediator.Options;

namespace Payeh.Mediator.Pipeline;

public class FallbackBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public FallbackBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<TRequest, CancellationToken, Task<TResponse>> next)
    {
        var configurator = _serviceProvider.GetRequiredService<MediatorConfigurator>();

        if (configurator.FallbackPolicies.TryGetValue(typeof(TRequest), out var fallbackFactory))
        {
            try
            {
                return await next(request, cancellationToken);
            }
            catch
            {
                // Use fallback value if the handler fails
                var fallbackValueFactory = (Func<TRequest, TResponse>)fallbackFactory;
                return fallbackValueFactory(request);
            }
        }

        // If no fallback policy, proceed with the next handler
        return await next(request, cancellationToken);
    }
}