namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehForbiddenResultException : PayehResultException
{
    public PayehForbiddenResultException(string message = "Access is forbidden.")
        : base("ERR_FORBIDDEN", message) { }
}