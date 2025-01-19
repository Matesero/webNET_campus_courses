using courses.Models.DTO;
using courses.Services;
using Microsoft.AspNetCore.Authorization;

namespace courses.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("register", Register);
 
        endpoints.MapPost("login", Login);
        
        return endpoints;
    }

    [Authorize]
    private static async Task<IResult> Register(
        UserRegisterModel request,
        UsersService usersService)
    {
        await usersService.Register(request.fullName,
            request.birthDate,
            request.email,
            request.password,
            request.confirmPassword);

        return Results.Ok();
    }
    private static async Task<IResult> Login(
        UserLoginModel request, 
        UsersService usersService,
        HttpContext context)
    {
        var response = await usersService.Login(request.email, request.password);

        context.Response.Cookies.Append("", response.token);
        
        return Results.Ok(response);
    }
}