namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehBadRequestResultException : PayehResultException
{
    public PayehBadRequestResultException(string message = "Bad request.")
        : base("ERR_BAD_REQUEST", message) { }
}