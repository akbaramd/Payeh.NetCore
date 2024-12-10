using Payeh.Mediator.Abstractions.Memento;

namespace Payeh.Mediator.Memento
{
    public class MementoBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMementoStore _mementoStore;

        public MementoBehavior(IMementoStore mementoStore)
        {
            _mementoStore = mementoStore;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            Func<TRequest, CancellationToken, Task<TResponse>> next)
        {
            // Capture the initial state
            var requestId = Guid.NewGuid();
            _mementoStore.Save(new RequestResponseMemento<TRequest,TResponse>(requestId, request));

            // Execute the request handler
            var response = await next(request, cancellationToken);

            // Capture the response state
            _mementoStore.UpdateResponse<TRequest,TResponse>(requestId, response);

            return response;
        }
    }
}