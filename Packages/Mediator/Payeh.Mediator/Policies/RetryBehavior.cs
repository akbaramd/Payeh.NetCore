using Payeh.Mediator.Options;

namespace Payeh.Mediator.Pipeline;

public class RetryBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public RetryBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<TRequest, CancellationToken, Task<TResponse>> next)
    {
        var configurator = _serviceProvider.GetRequiredService<MediatorConfigurator>();

        if (configurator.RetryPolicies.TryGetValue(typeof(TRequest), out var policyConfig))
        {
            dynamic config = policyConfig;
            var maxRetries = config.MaxRetries;
            var delay = config.Delay;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    return await next(request, cancellationToken);
                }
                catch when (attempt < maxRetries - 1)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }

        // If no retry policy, proceed with the next handler
        return await next(request, cancellationToken);
    }
    
}

public class RetryEventBehavior<TEvent> : IEventPipelineBehavior<TEvent>
    where TEvent : IEvent
{
    private readonly IServiceProvider _serviceProvider;

    public RetryEventBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(TEvent @event, CancellationToken cancellationToken, Func<TEvent, CancellationToken, Task> next)
    {
        var configurator = _serviceProvider.GetRequiredService<MediatorConfigurator>();

        if (configurator.RetryPolicies.TryGetValue(typeof(TEvent), out var policyConfig))
        {
            dynamic config = policyConfig;
            var maxRetries = config.MaxRetries;
            var delay = config.Delay;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    await next(@event, cancellationToken);
                    return; // Exit loop if successful
                }
                catch when (attempt < maxRetries - 1)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }

        // If no retry policy, proceed with the next handler
        await next(@event, cancellationToken);
    }
}