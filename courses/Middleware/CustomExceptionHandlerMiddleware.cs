using System.Net;
using System.Text.Json;
using courses.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace courses.Middleware;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public CustomExceptionHandlerMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var blackTokensService = scope.ServiceProvider.GetRequiredService<BlackTokensService>();

        var endpoint = context.GetEndpoint();
        
        var hasAuthorize = endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;

        if (hasAuthorize)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token) && await blackTokensService.CheckToken(token))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var result = JsonSerializer.Serialize(new
                {
                    status = (int)HttpStatusCode.Unauthorized,
                    message = "Unauthorized: Token is blacklisted"
                });
                await context.Response.WriteAsync(result);
                return;
            }
        }
        
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;
        var errors = new Dictionary<string, string[]>();

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(e => e.ErrorMessage).ToArray()
                    );
                result = JsonSerializer.Serialize(new
                {
                    title = "One or more validation errors occurred.",
                    status = (int)code,
                    errors = errors
                });
                break;
            
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            
            case InvalidPasswordException invalidPasswordException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                result = JsonSerializer.Serialize(new
                {
                    status = (int)code,
                    message = invalidPasswordException.Message
                });
                break;
            
            case NotFoundException notFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";

                result = JsonSerializer.Serialize(new
                {
                    status = (int)code,
                    message = notFoundException.Message
                });
                break;
        }
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        if (result == string.Empty)
        {
            result = JsonSerializer.Serialize(new { message = exception.Message });
        }
        
        return context.Response.WriteAsync(result);
    }
}