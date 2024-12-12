namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehAccessAuthorizeResultException : PayehResultException
{
    public PayehAccessAuthorizeResultException(string message = "Access authorization failed.")
        : base("ERR_ACCESS_AUTHORIZATION", message) { }
}