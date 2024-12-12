namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehInternalServerErrorResultException : PayehResultException
{
    public PayehInternalServerErrorResultException(string message = "An internal server error occurred.")
        : base("ERR_INTERNAL_SERVER_ERROR", message) { }
}