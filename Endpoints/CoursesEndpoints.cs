using courses.Extensions;
using courses.Models.DTO;
using courses.Models.enums;
using courses.Services;
using Microsoft.AspNetCore.Authorization;

namespace courses.Endpoints;

public static class CoursesEndpoints
{
    public static IEndpointRouteBuilder MapCoursesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/groups/{groupId}", CreateCourse).RequirePermissions(Permission.Create);
        
        return endpoints;
    }

    [Authorize]
    private static async Task<IResult> CreateCourse(
        Guid groupId,
        CreateCampusCourseModel request,
        CoursesService coursesService)
    {
        var response = await coursesService.Create(groupId, request.name, request.startYear, request.maximumStudentsCount, request.semester, request.requirements, request.annotations, request.mainTeacherId);
        
        return Results.Ok(response);
    }
}