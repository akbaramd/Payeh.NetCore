using FluentValidation.Results;

namespace Payeh.Result
{
    public partial class Result<TData>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public string ErrorMessage { get; private set; }
        public TData Data { get; private set; }

        // Core constructor
        protected Result(bool isSuccess, TData data = default, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        // Factory methods for basic success/failure
        public static Result<TData> Success(TData data) => new(true, data);
        public static Result<TData> Failure(string errorMessage) => new(false, default, errorMessage);
        
        // Implicit conversion from a service response (TData) to Result<TData>
        public static implicit operator Result<TData>(Result result)
        {
            
            return new Result<TData>(
                isSuccess:result.IsSuccess,
                errorMessage:result.ErrorMessage,
                errorCode:result.ErrorCode,
                validationErrors:result.ValidationErrors);
        }

        // Implicit conversion from Result<TData> to Result
        public static implicit operator Result(Result<TData> result)
        {
            return new Result(
                isSuccess:result.IsSuccess,
                errorCode:result.ErrorCode,
                validationErrors:result.ValidationErrors,
                errorMessage:result.ErrorMessage);
        }
    }

    public partial class Result : Result<Result>
    {
        public Result(bool isSuccess, Result data = null, string errorMessage = null)
            : base(isSuccess, data, errorMessage)
        {
        }

        public static Result Success() => new(true);
        public static Result Failure(string errorMessage) => new(false, null, errorMessage);
        
       
    }
}