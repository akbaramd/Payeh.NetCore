namespace Payeh.Mediator.Abstractions.Policies
{
    /// <summary>
    /// Represents a generic policy that can execute an action with defined strategies.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IPolicy<TResponse>
    {
        /// <summary>
        /// Executes the provided action according to the policy's strategy.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the action.</returns>
        Task<TResponse> ExecuteAsync(Func<Task<TResponse>> action);
    }
}