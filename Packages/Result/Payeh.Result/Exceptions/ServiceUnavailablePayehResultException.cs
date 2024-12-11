namespace Payeh.Result.Exceptions;

public class ServiceUnavailablePayehResultException : PayehResultException
{
    public ServiceUnavailablePayehResultException(string message = "Service is temporarily unavailable.")
        : base("ERR_SERVICE_UNAVAILABLE", message) { }
}