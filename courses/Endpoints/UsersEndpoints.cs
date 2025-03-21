﻿using courses.Extensions;
using courses.Models.DTO;
using courses.Models.enums;
using courses.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace courses.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/users", GetAllUsers).WithTags("Users");
        
        endpoints.MapGet("/roles", GetRoles).WithTags("Users");
        
        endpoints.MapPost("/registration", Register).WithTags("Account");
 
        endpoints.MapPost("/login", Login).WithTags("Account");
        
        endpoints.MapPost("/logout", Logout).WithTags("Account");
        
        endpoints.MapGet("/profile", GetUserProfile).WithTags("Account");
        
        endpoints.MapPut("/profile", EditUserProfile).WithTags("Account");
        
        return endpoints;
    }

    private static async Task<IResult> Register(
        UserRegisterModel request,
        IValidator<UserRegisterModel> validator,
        UsersService usersService)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var response =  await usersService.Register(
            request.fullName,
            request.birthDate,
            request.email,
            request.password);

        return Results.Ok(response);
    }
    
    private static async Task<IResult> Login(
        UserLoginModel request,
        IValidator<UserLoginModel> validator,
        UsersService usersService,
        HttpContext context)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var response = await usersService.Login(request.email, request.password);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> GetUserProfile(
        UsersService usersService,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        
        var response = await usersService.GetProfile(userId);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> EditUserProfile(
        EditUserProfileModel request,
        IValidator<EditUserProfileModel> validator,
        UsersService usersService,
        HttpContext context)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
         
        var userId = context.User.GetUserId();
        
        var response = await usersService.EditProfile(
            userId, 
            request.fullName, 
            request.birthDate);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> Logout(
        HttpRequest request,
        BlackTokensService blackTokensService)
    {
        var token = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        await blackTokensService.Add(token);
        
        return Results.Ok(new 
        {
            status = 200,
            message = "Logged Out"
        });
    }
    
    [Authorize]
    private static async Task<IResult> GetAllUsers(
        UsersService usersService,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        
        var response = await usersService.GetAll(userId);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> GetRoles(
        UsersService usersService,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        
        var response = await usersService.GetRoles(userId);
        
        return Results.Ok(response);
    }
}