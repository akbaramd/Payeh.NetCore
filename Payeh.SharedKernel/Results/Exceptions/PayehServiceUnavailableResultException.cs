namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehServiceUnavailableResultException : PayehResultException
{
    public PayehServiceUnavailableResultException(string message = "Service is temporarily unavailable.")
        : base("ERR_SERVICE_UNAVAILABLE", message) { }
}