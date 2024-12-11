namespace Payeh.Result.Exceptions;

public class InternalServerErrorPayehResultException : PayehResultException
{
    public InternalServerErrorPayehResultException(string message = "An internal server error occurred.")
        : base("ERR_INTERNAL_SERVER_ERROR", message) { }
}