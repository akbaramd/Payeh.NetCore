using System.Net;
using Payeh.Result.Exceptions;

namespace Payeh.Result.Middlewares
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
                ValidationPayehResultException validationEx => new PayehResult(false, validationEx.ValidationErrors, validationEx.ErrorCode, default, validationEx.Message),
                NotFoundPayehResultException notFoundEx => new PayehResult(false, notFoundEx.ErrorCode, default, notFoundEx.ErrorMessage),
                UnauthorizedPayehResultException unauthorizedEx => new PayehResult(false, unauthorizedEx.ErrorCode, default, unauthorizedEx.ErrorMessage),
                ForbiddenPayehResultException forbiddenEx => new PayehResult(false, forbiddenEx.ErrorCode, default, forbiddenEx.ErrorMessage),
                AccessAuthorizePayehResultException accessAuthEx => new PayehResult(false, accessAuthEx.ErrorCode, default, accessAuthEx.ErrorMessage),
                ConflictPayehResultException conflictEx => new PayehResult(false, conflictEx.ErrorCode, default, conflictEx.ErrorMessage),
                InternalServerErrorPayehResultException internalEx => new PayehResult(false, internalEx.ErrorCode, default, internalEx.ErrorMessage),
                ServiceUnavailablePayehResultException serviceEx => new PayehResult(false, serviceEx.ErrorCode, default, serviceEx.ErrorMessage),
                _ => new PayehResult(false, ex.ErrorCode, default, ex.ErrorMessage)
            };
        }

        private int GetStatusCode(PayehResultException ex)
        {
            return ex switch
            {
                NotFoundPayehResultException => StatusCodes.Status404NotFound,
                UnauthorizedPayehResultException => StatusCodes.Status401Unauthorized,
                ForbiddenPayehResultException => StatusCodes.Status403Forbidden,
                ValidationPayehResultException => StatusCodes.Status400BadRequest,
                AccessAuthorizePayehResultException => StatusCodes.Status401Unauthorized,
                ConflictPayehResultException => StatusCodes.Status409Conflict, // Handling ConflictException
                InternalServerErrorPayehResultException => StatusCodes.Status500InternalServerError, // Handling InternalServerErrorException
                ServiceUnavailablePayehResultException => StatusCodes.Status503ServiceUnavailable, // Handling ServiceUnavailableException
                _ => StatusCodes.Status500InternalServerError // default for other exceptions
            };
        }
    }
}
