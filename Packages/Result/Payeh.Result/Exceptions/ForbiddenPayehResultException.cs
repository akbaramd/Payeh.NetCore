namespace Payeh.Result.Exceptions;

public class ForbiddenPayehResultException : PayehResultException
{
    public ForbiddenPayehResultException(string message = "Access is forbidden.")
        : base("ERR_FORBIDDEN", message) { }
}