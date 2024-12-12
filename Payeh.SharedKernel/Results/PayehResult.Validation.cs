namespace Payeh.SharedKernel.Results
{
    public partial class PayehResult<TData>
    {
        public List<ValidationFailure> ValidationErrors { get; private set; } = new();

        // Constructor with validation errors
        protected PayehResult(bool isSuccess, List<ValidationFailure> validationErrors, string errorCode, TData data = default, string errorMessage = null)
            : this(isSuccess,errorCode, data, errorMessage)
        {
            ValidationErrors = validationErrors ?? new List<ValidationFailure>();
        }

        // Factory method for validation failure
        public static PayehResult<TData> FromValidationErrors(IEnumerable<ValidationFailure> errors)
        {
            return new PayehResult<TData>(false, errors.ToList(), "validation_error",default, "Validation failed");
        }
    }

    public partial class PayehResult
    {
        public List<ValidationFailure> ValidationErrors { get; private set; } = new();

        // Constructor with validation errors
        public PayehResult(bool isSuccess, List<ValidationFailure> validationErrors, string errorCode, PayehResult data = null, string errorMessage = null)
            : this(isSuccess, errorCode,data, errorMessage)
        {
            ValidationErrors = validationErrors ?? new List<ValidationFailure>();
        }

        // Factory method for validation failure
        public static PayehResult FromValidationErrors(IEnumerable<ValidationFailure> errors)
        {
            return new PayehResult(false, errors.ToList(),"valdiation_error", null, "Validation failed");
        }
    }
}