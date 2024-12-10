namespace Payeh.Mediator.Options;

public class LoggingOptions
{
    /// <summary>
    /// Indicates whether logging is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Configures the log level.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

   
        
}