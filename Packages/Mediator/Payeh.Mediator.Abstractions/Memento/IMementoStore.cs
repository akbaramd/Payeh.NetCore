namespace Payeh.Mediator.Abstractions.Memento
{
    public interface IMementoStore
    {
        void Save<TRequest, TResponse>(IRequestState<TRequest, TResponse> memento);
        void UpdateResponse<TRequest, TResponse>(Guid requestId, TResponse response);
        IRequestState<TRequest, TResponse> Get<TRequest, TResponse>(Guid id);
        IEnumerable<IRequestState<TRequest, TResponse>> GetAll<TRequest, TResponse>();
        

    }
}