using courses.Extensions;
using courses.Models.DTO;
using courses.Models.enums;
using courses.Services;
using Microsoft.AspNetCore.Authorization;

namespace courses.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/users", GetAllUsers).WithTags("Users").RequirePermissions(Permission.Read); // только админ или главный чел
        
        endpoints.MapPost("/registration", Register);
 
        endpoints.MapPost("/login", Login);
        
        endpoints.MapGet("/profile", GetUserProfile);
        
        endpoints.MapPut("/profile", EditUserProfile);
        
        return endpoints;
    }

    private static async Task<IResult> Register(
        UserRegisterModel request,
        UsersService usersService)
    {
        var response =  await usersService.Register(request.fullName,
            request.birthDate,
            request.email,
            request.password,
            request.confirmPassword);

        return Results.Ok(response);
    }
    private static async Task<IResult> Login(
        UserLoginModel request, 
        UsersService usersService,
        HttpContext context)
    {
        var response = await usersService.Login(request.email, request.password);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> GetUserProfile(
        UsersService usersService,
        HttpContext context)
    {
        var user = context.User.Claims.FirstOrDefault(
            c => c.Type == "userId");;
        
        if (user == null || string.IsNullOrEmpty(user.Value))
        {
            throw new Exception();
        }
        
        var response = await usersService.GetProfile(user.Value);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> EditUserProfile(
        EditUserProfileModel request,
        UsersService usersService,
        HttpContext context)
    {
        var user = context.User.Claims.FirstOrDefault(
            c => c.Type == "userId");;
        
        if (user == null || string.IsNullOrEmpty(user.Value))
        {
            throw new Exception();
        }
        
        var response = await usersService.EditProfile(user.Value, request.fullName, request.birthDate);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> GetAllUsers(
        UsersService usersService)
    {
        var response = await usersService.GetAll();
        
        return Results.Ok(response);
    }
}