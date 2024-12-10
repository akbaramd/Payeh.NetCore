namespace Payeh.Mediator.Abstractions.Memento
{
    public interface IRequestState<TRequest,TResponse>
    {
        public Guid RequestId { get; }
        public TRequest Request { get; }
        public TResponse Response { get; }
        
        void SetResponse(TResponse response);

    }
}