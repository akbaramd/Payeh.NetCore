namespace Payeh.Mediator.Abstractions.Events
{
    /// <summary>
    /// Represents a notification/event that can be published and handled by multiple handlers.
    /// Unlike a request, an event does not return a response.
    /// </summary>
    public interface IEvent { }
}