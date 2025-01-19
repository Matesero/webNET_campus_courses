using courses.Models.DTO;
using courses.Services;
using Microsoft.AspNetCore.Authorization;

namespace courses.Endpoints;

public static class GroupsEndpoints
{
    public static IEndpointRouteBuilder MapGroupsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/groups", GetAll);
        
        return endpoints;
    }

    [Authorize]
    private static async Task<IResult> GetAll(
        GroupsService groupsService)
    {
        var response = await groupsService.GetAll();
        
        return Results.Ok(response);
    }
}