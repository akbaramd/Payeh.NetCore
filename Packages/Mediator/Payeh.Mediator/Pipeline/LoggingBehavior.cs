
namespace Payeh.Mediator.Pipeline
{
    /// <summary>
    /// Logs information before and after handling a request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task<TResponse>> next)
        {
            _logger.LogInformation($"Handling request of type {typeof(TRequest).Name}");
            var response = await next(request, cancellationToken);
            _logger.LogInformation($"Handled request of type {typeof(TResponse).Name}");
            return response;
        }
    }
    
    
    /// <summary>
    /// Logs information before and after handling an event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public class LoggingEventBehavior<TEvent> : IEventPipelineBehavior<TEvent>
        where TEvent : IEvent
    {
        private readonly ILogger<LoggingEventBehavior<TEvent>> _logger;

        public LoggingEventBehavior(ILogger<LoggingEventBehavior<TEvent>> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TEvent @event, CancellationToken cancellationToken, Func<TEvent, CancellationToken, Task> next)
        {
            _logger.LogInformation($"Starting handling of event type: {typeof(TEvent).Name}");
            await next(@event, cancellationToken);
            _logger.LogInformation($"Completed handling of event type: {typeof(TEvent).Name}");
        }

       
    }
}