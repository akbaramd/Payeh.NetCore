using Payeh.Mediator.Abstractions.Memento;

namespace Payeh.Mediator.Options;

public class MementoOptions
{
    /// <summary>
    /// Indicates whether the memento behavior is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// A custom implementation of <see cref="IMementoStore"/>.
    /// If null, the default in-memory implementation will be used.
    /// </summary>
    public Type? CustomMementoStore { get; private set; }

    /// <summary>
    /// Specifies a custom implementation of <see cref="IMementoStore"/>.
    /// </summary>
    /// <typeparam name="TStore">The type of the custom memento store implementation.</typeparam>
    /// <returns>The updated <see cref="MementoOptions"/> instance.</returns>
    public MementoOptions AddMementoStore<TStore>() where TStore : IMementoStore
    {
        CustomMementoStore = typeof(TStore);
        return this;
    }
}