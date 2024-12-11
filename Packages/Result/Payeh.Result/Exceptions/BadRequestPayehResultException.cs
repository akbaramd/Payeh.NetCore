namespace Payeh.Result.Exceptions;

public class BadRequestPayehResultException : PayehResultException
{
    public BadRequestPayehResultException(string message = "Bad request.")
        : base("ERR_BAD_REQUEST", message) { }
}