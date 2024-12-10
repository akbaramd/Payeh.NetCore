using Microsoft.Extensions.DependencyInjection.Extensions;
using Payeh.Mediator.Abstractions.Memento;
using Payeh.Mediator.Memento;
using Payeh.Mediator.Options;

namespace Payeh.Mediator;

public static class MediatorServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatorConfigurator> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        var configurator = new MediatorConfigurator();
        configure(configurator);

        services.TryAddSingleton(configurator);

        // Register handlers from assemblies
        services.RegisterMediatorHandlers(configurator.AssembliesToScan.ToArray());

        // Register pipeline behaviors based on configuration
        RegisterPipelineBehaviors(services, configurator);

        // Register PolicyFactory
        services.TryAddSingleton<PolicyFactory>();

        // Register FallbackPolicy and RetryPolicy with generic capabilities
        services.AddSingleton(typeof(FallbackPolicy<>));
        services.AddSingleton(typeof(RetryPolicy<>));

        // Register core mediator services
        services.TryAddSingleton<IMediator, ServiceProviderMediator>();
        services.TryAddSingleton<RequestMediatorInvoker>();
        services.TryAddSingleton<MediatorEventInvoker>();
        services.TryAddSingleton<PolicyFactory>();

        

        return services;
    }

    private static void RegisterPipelineBehaviors(IServiceCollection services, MediatorConfigurator configurator)
    {
        services.AddSingleton(typeof(IRequestPipelineBehavior<,>), typeof(RetryBehavior<,>));
        services.AddSingleton(typeof(IEventPipelineBehavior<>), typeof(RetryEventBehavior<>));
        
        if (configurator.Logging.IsEnabled)
        {
            services.AddLogging(builder => builder.SetMinimumLevel(configurator.Logging.LogLevel));
            services.AddSingleton(typeof(IRequestPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddSingleton(typeof(IEventPipelineBehavior<>), typeof(LoggingEventBehavior<>));
        }

        if (configurator.Caching.IsEnabled)
        {
            services.AddMemoryCache();
            services.AddSingleton(typeof(IRequestPipelineBehavior<,>), typeof(CachingBehavior<,>));
        }

        if (configurator.Validation.IsEnabled)
        {
            services.AddSingleton(typeof(IRequestPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddSingleton(typeof(IEventPipelineBehavior<>), typeof(ValidationEventBehavior<>));
        }

        if (configurator.Memento.IsEnabled)
        {
            services.AddSingleton(typeof(IRequestPipelineBehavior<,>), typeof(MementoBehavior<,>));
            
            if (configurator.Memento.CustomMementoStore != null)
            {
                // Use custom memento store
                services.AddSingleton(typeof(IMementoStore), configurator.Memento.CustomMementoStore);
            }
            else
            {
                // Use default in-memory memento store
                services.AddSingleton<IMementoStore, MementoStore>();
            }

      
        }
        foreach (var behaviorType in configurator.PipelineBehaviors)
        {
            var pipelineInterface = typeof(IRequestPipelineBehavior<,>);
            var implementedInterfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == pipelineInterface);

            foreach (var implementedInterface in implementedInterfaces)
            {
                services.AddSingleton(implementedInterface, behaviorType);
            }
        }
    }
}