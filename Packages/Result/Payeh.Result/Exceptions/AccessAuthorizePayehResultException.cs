namespace Payeh.Result.Exceptions;

public class AccessAuthorizePayehResultException : PayehResultException
{
    public AccessAuthorizePayehResultException(string message = "Access authorization failed.")
        : base("ERR_ACCESS_AUTHORIZATION", message) { }
}