namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehConflictResultException : PayehResultException
{
    public PayehConflictResultException(string message = "Conflict occurred.")
        : base("ERR_CONFLICT", message) { }
}