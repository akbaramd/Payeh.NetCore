using System.Reflection;

namespace Payeh.Mediator.Options
{
    public class MediatorConfigurator
    {
        /// <summary>
        /// Assemblies to scan for handlers and behaviors.
        /// </summary>
        internal List<Assembly> AssembliesToScan { get; } = new List<Assembly>();
        internal Dictionary<Type, object> RetryPolicies { get; } = new Dictionary<Type, object>();
        internal Dictionary<Type, object> FallbackPolicies { get; } = new Dictionary<Type, object>();
        /// <summary>
        /// Custom pipeline behaviors to add.
        /// </summary>
        internal List<Type> PipelineBehaviors { get; } = new List<Type>();
        public MementoOptions Memento { get; } = new MementoOptions();
        /// <summary>
        /// Options for enabling or configuring logging.
        /// </summary>
        public LoggingOptions Logging { get; } = new LoggingOptions();

        /// <summary>
        /// Options for enabling or configuring caching.
        /// </summary>
        public CachingOptions Caching { get; } = new CachingOptions();

        /// <summary>
        /// Options for enabling or configuring validation.
        /// </summary>
        public ValidationOptions Validation { get; } = new ValidationOptions();

        /// <summary>
        /// Options for enabling or configuring fallback.
        /// </summary>
       
        public MediatorConfigurator AddRetryPolicy<TRequest>(int maxRetries, TimeSpan delay)
        {
            RetryPolicies[typeof(TRequest)] = new { MaxRetries = maxRetries, Delay = delay };
            return this;
        }

        public MediatorConfigurator AddFallbackPolicy<TRequest>(Func<TRequest, object> fallbackValueFactory)
        {
            FallbackPolicies[typeof(TRequest)] = fallbackValueFactory;
            return this;
        }
        /// <summary>
        /// Adds assemblies to scan for request handlers and event handlers.
        /// </summary>
        /// <param name="assemblies">Assemblies to scan.</param>
        public MediatorConfigurator AddAssemblies(params Assembly[] assemblies)
        {
            AssembliesToScan.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Adds a custom pipeline behavior to the mediator.
        /// </summary>
        /// <typeparam name="TBehavior">The type of the behavior.</typeparam>
        public MediatorConfigurator AddPipelineBehavior<TBehavior>() where TBehavior : IPipelineBehavior
        {
            PipelineBehaviors.Add(typeof(TBehavior));
            return this;
        }
    }
}
