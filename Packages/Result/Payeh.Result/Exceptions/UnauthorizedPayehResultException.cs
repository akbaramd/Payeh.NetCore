namespace Payeh.Result.Exceptions;

public class UnauthorizedPayehResultException : PayehResultException
{
    public UnauthorizedPayehResultException(string message = "Unauthorized access.")
        : base("ERR_UNAUTHORIZED", message) { }
}