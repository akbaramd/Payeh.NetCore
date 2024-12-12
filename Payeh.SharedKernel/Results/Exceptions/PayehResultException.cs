using Payeh.SharedKernel.Exceptions;

namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehResultException : PayehException
{
    public string ErrorCode { get; }
    public string ErrorMessage { get; }

    // Constructor with default error code and message
    public PayehResultException(string errorCode, string errorMessage)
        : base(errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}

