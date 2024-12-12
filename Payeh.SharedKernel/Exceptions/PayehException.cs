namespace Payeh.SharedKernel.Exceptions;

/// <summary>
/// Represents a custom exception for the Payeh application.
/// </summary>
public class PayehException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PayehException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public PayehException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PayehException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public PayehException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}