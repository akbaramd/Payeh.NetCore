namespace Payeh.SharedKernel.Results
{
    public partial class PayehResult<TData>
    {
        public string ErrorCode { get; private set; }

        // Constructor with error code
        protected PayehResult(bool isSuccess, string errorCode, TData data = default, string errorMessage = null)
            : this(isSuccess, data, errorMessage)
        {
            ErrorCode = errorCode;
        }

        // Factory method for failure with error code
        public static PayehResult<TData> Failure(string errorCode, string errorMessage)
        {
            return new PayehResult<TData>(false, errorCode, default, errorMessage);
        }
        
        // Predefined behaviors
        public static PayehResult<TData> NotFound(string errorMessage = "Resource not found.")
        {
            return Failure("ERR_NOT_FOUND", errorMessage);
        }

        public static PayehResult<TData> AlreadyExists(string errorMessage = "Resource already exists.")
        {
            return Failure("ERR_ALREADY_EXISTS", errorMessage);
        }

        public static PayehResult<TData> Conflict(string errorMessage = "Conflict occurred.")
        {
            return Failure("ERR_CONFLICT", errorMessage);
        }

        public static PayehResult<TData> Unauthorized(string errorMessage = "Unauthorized access.")
        {
            return Failure("ERR_UNAUTHORIZED", errorMessage);
        }

        public static PayehResult<TData> Forbidden(string errorMessage = "Access is forbidden.")
        {
            return Failure("ERR_FORBIDDEN", errorMessage);
        }

        public static PayehResult<TData> BadRequest(string errorMessage = "Invalid request.")
        {
            return Failure("ERR_BAD_REQUEST", errorMessage);
        }
    }

    public partial class PayehResult 
    {
        public string ErrorCode { get; private set; }

        // Constructor with error code
        public PayehResult(bool isSuccess, string errorCode, PayehResult data = null, string errorMessage = null)
            : this(isSuccess, data, errorMessage)
        {
            ErrorCode = errorCode;
        }

        // Factory method for failure with error code
        public static PayehResult Failure(string errorCode, string errorMessage)
        {
            return new PayehResult(false, errorCode, null, errorMessage);
        }
        
        
        // Predefined behaviors
        public new static PayehResult NotFound(string errorMessage = "Resource not found.")
        {
            return Failure("ERR_NOT_FOUND", errorMessage);
        }

        public static PayehResult AlreadyExists(string errorMessage = "Resource already exists.")
        {
            return Failure("ERR_ALREADY_EXISTS", errorMessage);
        }

        public static PayehResult Conflict(string errorMessage = "Conflict occurred.")
        {
            return Failure("ERR_CONFLICT", errorMessage);
        }

        public static PayehResult Unauthorized(string errorMessage = "Unauthorized access.")
        {
            return Failure("ERR_UNAUTHORIZED", errorMessage);
        }

        public static PayehResult Forbidden(string errorMessage = "Access is forbidden.")
        {
            return Failure("ERR_FORBIDDEN", errorMessage);
        }

        public static PayehResult BadRequest(string errorMessage = "Invalid request.")
        {
            return Failure("ERR_BAD_REQUEST", errorMessage);
        }
    }
}