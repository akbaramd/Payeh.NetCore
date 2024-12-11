namespace Payeh.Result.Exceptions;

public class NotFoundPayehResultException : PayehResultException
{
    public NotFoundPayehResultException(string message = "Resource not found.")
        : base("ERR_NOT_FOUND", message) { }
}