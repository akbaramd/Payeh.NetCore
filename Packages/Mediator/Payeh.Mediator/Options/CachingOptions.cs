namespace Payeh.Mediator.Options;

public class CachingOptions
{
    /// <summary>
    /// Indicates whether caching is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// The default cache duration in seconds.
    /// </summary>
    public int DefaultCacheDuration { get; set; } = 300;

}