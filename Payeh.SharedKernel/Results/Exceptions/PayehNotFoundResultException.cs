namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehNotFoundResultException : PayehResultException
{
    public PayehNotFoundResultException(string message = "Resource not found.")
        : base("ERR_NOT_FOUND", message) { }
}