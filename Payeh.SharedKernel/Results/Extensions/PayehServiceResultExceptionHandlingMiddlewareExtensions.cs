using Payeh.SharedKernel.Results.Middlewares;

namespace Payeh.SharedKernel.Results.Extensions
{
    public static class PayehServiceResultExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UsePayehServiceResultExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}