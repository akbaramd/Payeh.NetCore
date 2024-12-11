using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Payeh.Mediator.Pipeline
{
    /// <summary>
    /// Validates the request using FluentValidation before passing it to the next behavior or handler.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest> _validator;

        public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>> logger, IValidator<TRequest> validator)
        {
            _logger = logger;
            _validator = validator;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task<TResponse>> next)
        {
            // Validate using the provided validator
            if (_validator != null)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning($"Validation failed for request {typeof(TRequest).Name}: {string.Join(", ", validationResult.Errors)}");
                    throw new ValidationException(validationResult.Errors);
                }
            }

            // Proceed to the next handler
            return await next(request, cancellationToken);
        }
    }
    
    /// <summary>
    /// Validates the event using FluentValidation before passing it to the next behavior or handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public class ValidationEventBehavior<TEvent> : IEventPipelineBehavior<TEvent>
        where TEvent : IEvent
    {
        private readonly ILogger<ValidationEventBehavior<TEvent>> _logger;
        private readonly IValidator<TEvent> _validator;

        public ValidationEventBehavior(ILogger<ValidationEventBehavior<TEvent>> logger, IValidator<TEvent> validator)
        {
            _logger = logger;
            _validator = validator;
        }

        public async Task Handle(TEvent @event, CancellationToken cancellationToken, Func<TEvent, CancellationToken, Task> next)
        {
            // Validate using the provided validator
            if (_validator != null)
            {
                var validationResult = await _validator.ValidateAsync(@event, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning($"Validation failed for event {typeof(TEvent).Name}: {string.Join(", ", validationResult.Errors)}");
                    throw new ValidationException(validationResult.Errors);
                }
            }

            // Proceed to the next handler
            await next(@event, cancellationToken);
        }
    }
}
