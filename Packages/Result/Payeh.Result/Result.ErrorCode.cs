namespace Payeh.Result
{
    public partial class Result<TData>
    {
        public string ErrorCode { get; private set; }

        // Constructor with error code
        protected Result(bool isSuccess, string errorCode, TData data = default, string errorMessage = null)
            : this(isSuccess, data, errorMessage)
        {
            ErrorCode = errorCode;
        }

        // Factory method for failure with error code
        public static Result<TData> Failure(string errorCode, string errorMessage)
        {
            return new Result<TData>(false, errorCode, default, errorMessage);
        }
        
        // Predefined behaviors
        public static Result<TData> NotFound(string errorMessage = "Resource not found.")
        {
            return Failure("ERR_NOT_FOUND", errorMessage);
        }

        public static Result<TData> AlreadyExists(string errorMessage = "Resource already exists.")
        {
            return Failure("ERR_ALREADY_EXISTS", errorMessage);
        }

        public static Result<TData> Conflict(string errorMessage = "Conflict occurred.")
        {
            return Failure("ERR_CONFLICT", errorMessage);
        }

        public static Result<TData> Unauthorized(string errorMessage = "Unauthorized access.")
        {
            return Failure("ERR_UNAUTHORIZED", errorMessage);
        }

        public static Result<TData> Forbidden(string errorMessage = "Access is forbidden.")
        {
            return Failure("ERR_FORBIDDEN", errorMessage);
        }

        public static Result<TData> BadRequest(string errorMessage = "Invalid request.")
        {
            return Failure("ERR_BAD_REQUEST", errorMessage);
        }
    }

    public partial class Result 
    {
        public string ErrorCode { get; private set; }

        // Constructor with error code
        public Result(bool isSuccess, string errorCode, Result data = null, string errorMessage = null)
            : this(isSuccess, data, errorMessage)
        {
            ErrorCode = errorCode;
        }

        // Factory method for failure with error code
        public static Result Failure(string errorCode, string errorMessage)
        {
            return new Result(false, errorCode, null, errorMessage);
        }
        
        
        // Predefined behaviors
        public new static Result NotFound(string errorMessage = "Resource not found.")
        {
            return Failure("ERR_NOT_FOUND", errorMessage);
        }

        public static Result AlreadyExists(string errorMessage = "Resource already exists.")
        {
            return Failure("ERR_ALREADY_EXISTS", errorMessage);
        }

        public static Result Conflict(string errorMessage = "Conflict occurred.")
        {
            return Failure("ERR_CONFLICT", errorMessage);
        }

        public static Result Unauthorized(string errorMessage = "Unauthorized access.")
        {
            return Failure("ERR_UNAUTHORIZED", errorMessage);
        }

        public static Result Forbidden(string errorMessage = "Access is forbidden.")
        {
            return Failure("ERR_FORBIDDEN", errorMessage);
        }

        public static Result BadRequest(string errorMessage = "Invalid request.")
        {
            return Failure("ERR_BAD_REQUEST", errorMessage);
        }
    }
}