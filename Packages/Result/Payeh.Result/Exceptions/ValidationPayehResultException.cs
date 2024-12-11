namespace Payeh.Result.Exceptions;

public class ValidationPayehResultException : PayehResultException
{
    public List<ValidationFailure> ValidationErrors { get; }

    public ValidationPayehResultException(IEnumerable<ValidationFailure> errors)
        : base("ERR_VALIDATION", "Validation failed")
    {
        ValidationErrors = errors.ToList();
    }
}