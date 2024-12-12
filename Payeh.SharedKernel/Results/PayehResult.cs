namespace Payeh.SharedKernel.Results
{
    public partial class PayehResult<TData>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public string ErrorMessage { get; private set; }
        public TData Data { get; private set; }

        // Core constructor
        protected PayehResult(bool isSuccess, TData data = default, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        // Factory methods for basic success/failure
        public static PayehResult<TData> Success(TData data) => new(true, data);
        public static PayehResult<TData> Failure(string errorMessage) => new(false, default, errorMessage);
        
        // Implicit conversion from a service response (TData) to Result<TData>
        public static implicit operator PayehResult<TData>(PayehResult payehResult)
        {
            
            return new PayehResult<TData>(
                isSuccess:payehResult.IsSuccess,
                errorMessage:payehResult.ErrorMessage,
                errorCode:payehResult.ErrorCode,
                validationErrors:payehResult.ValidationErrors);
        }

        // Implicit conversion from Result<TData> to Result
        public static implicit operator PayehResult(PayehResult<TData> payehResult)
        {
            return new PayehResult(
                isSuccess:payehResult.IsSuccess,
                errorCode:payehResult.ErrorCode,
                validationErrors:payehResult.ValidationErrors,
                errorMessage:payehResult.ErrorMessage);
        }
    }

    public partial class PayehResult : PayehResult<PayehResult>
    {
        public PayehResult(bool isSuccess, PayehResult data = null, string errorMessage = null)
            : base(isSuccess, data, errorMessage)
        {
        }

        public static PayehResult Success() => new(true);
        public static PayehResult Failure(string errorMessage) => new(false, null, errorMessage);
        
       
    }
}