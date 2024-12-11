using Payeh.Result.Middlewares;

namespace Payeh.Result
{
    public static class PayehServiceResultExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UsePayehServiceResultExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}