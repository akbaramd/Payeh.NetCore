namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehUnauthorizedResultException : PayehResultException
{
    public PayehUnauthorizedResultException(string message = "Unauthorized access.")
        : base("ERR_UNAUTHORIZED", message) { }
}