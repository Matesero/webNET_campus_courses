using courses.Extensions;
using courses.Models.DTO;
using courses.Models.enums;
using courses.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace courses.Endpoints;

public static class GroupsEndpoints
{
    public static IEndpointRouteBuilder MapGroupsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groups = endpoints.MapGroup("/groups").RequireAuthorization().WithTags("Group");

        groups.MapGet("", GetAll);
        
        groups.MapPost("", CreateGroup).RequirePermissions(Permission.Create);
        
        groups.MapPut("/{id}", EditGroup).RequirePermissions(Permission.Update);
        
        groups.MapDelete("/{id}", DeleteGroup).RequirePermissions(Permission.Delete);

        groups.MapGet("/{id}", GetListCourses);
        
        endpoints.MapGet("/report", Report).RequirePermissions(Permission.Read).WithTags("Report");
        
        return endpoints;
    }
    
    [Authorize]
    private static async Task<IResult> GetAll(
        GroupsService groupsService)
    {
        var response = await groupsService.GetAll();
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> CreateGroup(
        CreateCampusGroupModel request,
        IValidator<CreateCampusGroupModel> validator,
        GroupsService groupsService)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var response = await groupsService.Create(request.name);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> EditGroup(
        Guid id,
        EditCampusGroupModel request,
        IValidator<EditCampusGroupModel> validator,
        GroupsService groupsService)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var response = await groupsService.Edit(id, request.name);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> DeleteGroup(
        Guid id,
        GroupsService groupsService)
    {
        await groupsService.Delete(id);
        
        return Results.Ok();
    }
    
    [Authorize]
    private static async Task<IResult> GetListCourses(
        Guid id,
        GroupsService groupsService)
    {
        var response = await groupsService.GetCourses(id);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> Report(
        [FromQuery] Semesters? semester,
        [FromQuery] Guid[]? campusGroupIds,
        GroupsService groupsService)
    {
        var response = await groupsService.GetReports(semester, campusGroupIds.ToList());
        
        return Results.Ok(response);
    }
}