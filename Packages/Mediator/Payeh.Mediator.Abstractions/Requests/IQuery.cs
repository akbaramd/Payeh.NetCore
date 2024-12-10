namespace Payeh.Mediator.Abstractions.Requests
{
    /// <summary>
    /// Marker interface for queries, which typically retrieve data without changing the system state.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    public interface IQuery<TResponse> : IRequest<TResponse> { }
}