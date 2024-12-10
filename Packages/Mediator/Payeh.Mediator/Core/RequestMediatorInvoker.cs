namespace Payeh.Mediator.Core
{
    public class RequestMediatorInvoker
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestMediatorInvoker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Invoke<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            // Determine the specific handler type
            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            // Resolve the handler
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
                throw new InvalidOperationException($"No handler found for {requestType.Name}");

            // Resolve pipeline behaviors
            var pipelineBehaviorType = typeof(IRequestPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
            var behaviors = _serviceProvider.GetServices(pipelineBehaviorType).ToList();

            // Build the final handler invocation
            Func<IRequest<TResponse>, CancellationToken, Task<TResponse>> invokeHandler =
                (req, ct) =>
                {
                    var method = handlerType.GetMethod("Handle");
                    if (method == null)
                        throw new InvalidOperationException($"Handler {handler.GetType().Name} does not implement Handle method.");

                    return (Task<TResponse>)method.Invoke(handler, new object[] { req, ct });
                };

            // Wrap with pipeline behaviors in reverse order
            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                var next = invokeHandler;
                invokeHandler = (req, ct) =>
                {
                    var method = pipelineBehaviorType.GetMethod("Handle");
                    if (method == null)
                        throw new InvalidOperationException($"Pipeline behavior {behavior.GetType().Name} does not implement Handle method.");

                    return (Task<TResponse>)method.Invoke(behavior, new object[] { req, ct, next });
                };
            }

            // Invoke the pipeline chain
            return await invokeHandler(request, cancellationToken);
        }
    }
}
