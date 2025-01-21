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
        // endpoints.MapGet("/couriers/{id}/details", GetCourseDetails).RequirePermissions(Permission.Create);
        
        endpoints.MapPost("/groups/{groupId}", CreateCourse).RequirePermissions(Permission.Create);
        
        endpoints.MapDelete("/courses/{id}", DeleteCourse).RequirePermissions(Permission.Delete);
        
        endpoints.MapPost("/courses/{id}/notification", CreateNotification).RequirePermissions(Permission.Create);
        
        // endpoints.MapPut("/courses/{id}", EditCourse).RequirePermissions(Permission.Update);

        endpoints.MapPost("/courses/{id}/sign-up", SignUpCourse);
        
        endpoints.MapGet("/courses/my", GetMyCourses);
        
        endpoints.MapGet("/courses/teaching", GetTeachingCourses);
        
        return endpoints;
    }

    [Authorize]
    private static async Task<IResult> CreateCourse(
        Guid groupId,
        CreateCampusCourseModel request,
        CoursesService coursesService)
    {
        var response = await coursesService.Create(
            groupId, 
            request.name, 
            request.startYear, 
            request.maximumStudentsCount, 
            request.semester, 
            request.requirements, 
            request.annotations, 
            request.mainTeacherId);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> DeleteCourse(
        Guid id,
        CoursesService coursesService)
    {
        var response = await coursesService.Delete(id);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> SignUpCourse(
        Guid courseId,
        CoursesService coursesService,
        HttpContext context)
    {
        var userId = context.User.Claims.FirstOrDefault(
            c => c.Type == "userId");;
        
        if (userId == null || string.IsNullOrEmpty(userId.Value))
        {
            throw new Exception();
        }
        
        await coursesService.SignUp(userId.Value, courseId);
        
        return Results.Ok();
    }

    [Authorize]
    private static async Task<IResult> GetMyCourses(
        CoursesService coursesService,
        HttpContext context)
    {
        var userId = context.User.Claims.FirstOrDefault(
            c => c.Type == "userId");
        ;

        if (userId == null || string.IsNullOrEmpty(userId.Value))
        {
            throw new Exception();
        }

        var response = await coursesService.GetMyCourses(userId.Value);

        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> GetTeachingCourses(
        CoursesService coursesService,
        HttpContext context)
    {
        var userId = context.User.Claims.FirstOrDefault(
            c => c.Type == "userId");;
        
        if (userId == null || string.IsNullOrEmpty(userId.Value))
        {
            throw new Exception();
        }
        
        var response = await coursesService.GetTeachingCourses(userId.Value);

        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> CreateNotification(
        Guid id, 
        CampusCourseNotificationModel request,
        CoursesService coursesService)
    {
        await coursesService.CreateNotification(id, request.text, request.isImportant);
        
        return Results.Ok();
    }

    // [Authorize]
    // private static async Task<IResult> EditCourse(
    //     Guid id,
    //     EditCampusCourseModel request,
    //     CoursesService coursesService)
    // {
    //     await coursesService.Edit(
    //         request.name, 
    //         request.startYear, 
    //         request.maximumStudentsCount, 
    //         request.semester, 
    //         request.requirements, 
    //         request.annotations, 
    //         request.mainTeacherId);
    //     ;
    //     
    //     return Results.Ok();
    // }
}