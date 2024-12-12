namespace Payeh.SharedKernel.Results.Exceptions;

public class PayehValidationResultException : PayehResultException
{
    public List<ValidationFailure> ValidationErrors { get; }

    public PayehValidationResultException(IEnumerable<ValidationFailure> errors)
        : base("ERR_VALIDATION", "Validation failed")
    {
        ValidationErrors = errors.ToList();
    }
}