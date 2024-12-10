namespace Payeh.Mediator.Abstractions.Requests
{
    /// <summary>
    /// Marker interface for commands, which typically change the system state.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    public interface ICommand<TResponse> : IRequest<TResponse> { }
}