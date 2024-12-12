using System.Net;
using Payeh.SharedKernel.Results.Exceptions;

namespace Payeh.SharedKernel.Results.Middlewares
{
    internal class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (PayehResultException ex)
            {
                _logger.LogError(ex, "An exception occurred");
                var result = HandleException(ex);
                httpContext.Response.StatusCode = GetStatusCode(ex);
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsJsonAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                var result = new PayehResult(false, "ERR_INTERNAL_SERVER_ERROR", default, "An unexpected error occurred.");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsJsonAsync(result);
            }
        }

        private PayehResult HandleException(PayehResultException ex)
        {
            // Mapping exceptions to ServiceResult using constructor
            return ex switch
            {
                PayehValidationResultException validationEx => new PayehResult(false, validationEx.ValidationErrors, validationEx.ErrorCode, default, validationEx.Message),
                PayehNotFoundResultException notFoundEx => new PayehResult(false, notFoundEx.ErrorCode, default, notFoundEx.ErrorMessage),
                PayehUnauthorizedResultException unauthorizedEx => new PayehResult(false, unauthorizedEx.ErrorCode, default, unauthorizedEx.ErrorMessage),
                PayehForbiddenResultException forbiddenEx => new PayehResult(false, forbiddenEx.ErrorCode, default, forbiddenEx.ErrorMessage),
                PayehAccessAuthorizeResultException accessAuthEx => new PayehResult(false, accessAuthEx.ErrorCode, default, accessAuthEx.ErrorMessage),
                PayehConflictResultException conflictEx => new PayehResult(false, conflictEx.ErrorCode, default, conflictEx.ErrorMessage),
                PayehInternalServerErrorResultException internalEx => new PayehResult(false, internalEx.ErrorCode, default, internalEx.ErrorMessage),
                PayehServiceUnavailableResultException serviceEx => new PayehResult(false, serviceEx.ErrorCode, default, serviceEx.ErrorMessage),
                _ => new PayehResult(false, ex.ErrorCode, default, ex.ErrorMessage)
            };
        }

        private int GetStatusCode(PayehResultException ex)
        {
            return ex switch
            {
                PayehNotFoundResultException => StatusCodes.Status404NotFound,
                PayehUnauthorizedResultException => StatusCodes.Status401Unauthorized,
                PayehForbiddenResultException => StatusCodes.Status403Forbidden,
                PayehValidationResultException => StatusCodes.Status400BadRequest,
                PayehAccessAuthorizeResultException => StatusCodes.Status401Unauthorized,
                PayehConflictResultException => StatusCodes.Status409Conflict, // Handling ConflictException
                PayehInternalServerErrorResultException => StatusCodes.Status500InternalServerError, // Handling InternalServerErrorException
                PayehServiceUnavailableResultException => StatusCodes.Status503ServiceUnavailable, // Handling ServiceUnavailableException
                _ => StatusCodes.Status500InternalServerError // default for other exceptions
            };
        }
    }
}
