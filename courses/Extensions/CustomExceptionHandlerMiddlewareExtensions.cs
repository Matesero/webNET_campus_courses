using courses.Middleware;

namespace courses.Extensions;

public static class CustomExceptionHandlerMiddlewareExtensions
{
    public static void UseMiddlewareHandlerException(this IApplicationBuilder app)
    {
        app.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}