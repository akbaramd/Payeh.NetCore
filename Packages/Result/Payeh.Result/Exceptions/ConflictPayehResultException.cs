namespace Payeh.Result.Exceptions;

public class ConflictPayehResultException : PayehResultException
{
    public ConflictPayehResultException(string message = "Conflict occurred.")
        : base("ERR_CONFLICT", message) { }
}