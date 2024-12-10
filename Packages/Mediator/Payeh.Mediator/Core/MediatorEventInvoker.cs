namespace Payeh.Mediator.Core
{
    public class MediatorEventInvoker
    {
        private readonly IServiceProvider _serviceProvider;

        public MediatorEventInvoker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(IEvent @event, CancellationToken cancellationToken)
        {
            // Get the type of the event
            var eventType = @event.GetType();

            // Resolve all event handlers for this event type
            var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var eventHandlers = _serviceProvider.GetServices(eventHandlerType).ToList();

            if (!eventHandlers.Any())
            {
                throw new InvalidOperationException($"No handlers found for event type {eventType.Name}");
            }

            // Define the final handler invocation chain
            Func<IEvent, CancellationToken, Task> invokeHandlers = async (evt, ct) =>
            {
                foreach (var eventHandler in eventHandlers)
                {
                    var handleMethod = eventHandlerType.GetMethod("Handle");
                    if (handleMethod == null)
                    {
                        throw new InvalidOperationException(
                            $"Handler {eventHandler.GetType().Name} does not implement a Handle method.");
                    }

                    var task = (Task)handleMethod.Invoke(eventHandler, new object[] { evt, ct });
                    await task;
                }
            };

            // Resolve all pipeline behaviors for this event type
            var pipelineBehaviorType = typeof(IEventPipelineBehavior<>).MakeGenericType(eventType);
            var behaviors = _serviceProvider.GetServices(pipelineBehaviorType).ToList();

            // Wrap the final handler invocation with pipeline behaviors
            Func<IEvent, CancellationToken, Task> invokePipeline = invokeHandlers;

            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                var currentBehavior = behavior;
                var handleMethod = pipelineBehaviorType.GetMethod("Handle");

                if (handleMethod == null)
                {
                    throw new InvalidOperationException(
                        $"Pipeline behavior {behavior.GetType().Name} does not implement a Handle method.");
                }

                var next = invokePipeline;
                invokePipeline = (evt, ct) => 
                    (Task)handleMethod.Invoke(currentBehavior, new object[] { evt, ct, next });
            }

            // Invoke the full pipeline
            await invokePipeline(@event, cancellationToken);
        }
    }
}
