using FluentValidation.Results;

namespace Payeh.Result
{
    public partial class Result<TData>
    {
        public List<ValidationFailure> ValidationErrors { get; private set; } = new();

        // Constructor with validation errors
        protected Result(bool isSuccess, List<ValidationFailure> validationErrors, string errorCode, TData data = default, string errorMessage = null)
            : this(isSuccess,errorCode, data, errorMessage)
        {
            ValidationErrors = validationErrors ?? new List<ValidationFailure>();
        }

        // Factory method for validation failure
        public static Result<TData> FromValidationErrors(IEnumerable<ValidationFailure> errors)
        {
            return new Result<TData>(false, errors.ToList(), "validation_error",default, "Validation failed");
        }
    }

    public partial class Result
    {
        public List<ValidationFailure> ValidationErrors { get; private set; } = new();

        // Constructor with validation errors
        public Result(bool isSuccess, List<ValidationFailure> validationErrors, string errorCode, Result data = null, string errorMessage = null)
            : this(isSuccess, errorCode,data, errorMessage)
        {
            ValidationErrors = validationErrors ?? new List<ValidationFailure>();
        }

        // Factory method for validation failure
        public static Result FromValidationErrors(IEnumerable<ValidationFailure> errors)
        {
            return new Result(false, errors.ToList(),"valdiation_error", null, "Validation failed");
        }
    }
}