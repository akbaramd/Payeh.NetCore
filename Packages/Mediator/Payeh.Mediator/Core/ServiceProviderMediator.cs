namespace Payeh.Mediator.Core
{
    internal class ServiceProviderMediator : IMediator
    {
        private readonly RequestMediatorInvoker _requestInvoker;
        private readonly MediatorEventInvoker _eventInvoker;

        public ServiceProviderMediator(RequestMediatorInvoker requestInvoker, MediatorEventInvoker eventInvoker)
        {
            _requestInvoker = requestInvoker;
            _eventInvoker = eventInvoker;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return await _requestInvoker.Invoke<TResponse>(request, cancellationToken);
        }

        public async Task Publish(IEvent @event, CancellationToken cancellationToken = default)
        {
            await _eventInvoker.Invoke(@event, cancellationToken);
        }
    }
}