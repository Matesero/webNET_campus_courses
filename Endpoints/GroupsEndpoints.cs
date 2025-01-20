using courses.Extensions;
using courses.Models.DTO;
using courses.Models.enums;
using courses.Services;
using Microsoft.AspNetCore.Authorization;

namespace courses.Endpoints;

public static class GroupsEndpoints
{
    public static IEndpointRouteBuilder MapGroupsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/groups", GetAll);
        
        endpoints.MapPost("/groups", CreateGroup).RequirePermissions(Permission.Create);
        
        endpoints.MapPut("/groups/{id}", EditGroup).RequirePermissions(Permission.Update);
        
        endpoints.MapDelete("/groups/{id}", DeleteGroup).RequirePermissions(Permission.Delete);

        endpoints.MapGet("/groups/{id}", GetListCourses);
        
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
        GroupsService groupsService)
    {
        await groupsService.Create(request.name);
        
        return Results.Ok();
    }
    
    [Authorize]
    private static async Task<IResult> EditGroup(
        Guid id,
        EditCampusGroupModel request,
        GroupsService groupsService)
    {
        await groupsService.Edit(id, request.name);
        
        return Results.Ok();
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
}