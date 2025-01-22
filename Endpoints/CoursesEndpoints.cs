using System.ComponentModel.DataAnnotations;
using courses.Extensions;
using courses.Models.DTO;
using courses.Models.enums;
using courses.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace courses.Endpoints;

public static class CoursesEndpoints
{
    public static IEndpointRouteBuilder MapCoursesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/courses/{id}/details", GetCourseDetailedInfo);
        
        endpoints.MapPost("/groups/{groupId}", CreateCourse).RequirePermissions(Permission.Create);
        
        endpoints.MapPost("/courses/{id}/status", EditCoursesStatus).RequirePermissions(Permission.Update);
        
        endpoints.MapPost("/courses/{id}/requirements-and-annotations", EditCoursesRequirementsAndAnnotations).RequirePermissions(Permission.Update);
        
        endpoints.MapDelete("/courses/{id}", DeleteCourse).RequirePermissions(Permission.Delete);
        
        endpoints.MapPost("/courses/{id}/notification", CreateNotification).RequirePermissions(Permission.Create);
        
        endpoints.MapPut("/courses/{id}", EditCourse).RequirePermissions(Permission.Update);

        endpoints.MapPost("/courses/{id}/sign-up", SignUpCourse);
        
        endpoints.MapPost("/courses/{id}/teacher", AddTeacherToCourse).RequirePermissions(Permission.Update);
        
        endpoints.MapGet("/courses/my", GetMyCourses);
        
        endpoints.MapGet("/courses/teaching", GetTeachingCourses);
        
        endpoints.MapGet("/courses/list", GetFilteredCourses);
        
        endpoints.MapPost("/courses/{id}/student-status/{studentId}", ChangeStudentStatus).RequirePermissions(Permission.Update);
        
        endpoints.MapPost("/courses/{id}/marks/{studentId}", ChangeStudentMark).RequirePermissions(Permission.Update);
        
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

    [Authorize]
    private static async Task<IResult> GetCourseDetailedInfo(
        CoursesService coursesService,
        Guid id)
    {
        var response = await coursesService.GetDetailedInfo(id);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> GetFilteredCourses(
        CoursesService coursesService,
        [FromQuery] SortList? sort,
        [FromQuery] string? search,
        [FromQuery] bool? hasPlacesAndOpen,
        [FromQuery] Semesters? semester,
        [FromQuery, Range(1, int.MaxValue)] int page,
        [FromQuery, Range(1, int.MaxValue)] int pageSize)
    {
        var response = await coursesService.GetFilteredCourses(sort, search, hasPlacesAndOpen, semester, page, pageSize);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> EditCoursesStatus(
        CoursesService coursesService,
        EditCourseStatusModel request,
        Guid id)
    {
        var response = await coursesService.EditCoursesStatus(id, request.status);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> EditCoursesRequirementsAndAnnotations(
        CoursesService coursesService,
        EditCampusCourseRequirementsAndAnnotationsModel request,
        Guid id)
    {
        var response = await coursesService.EditCoursesRequirementsAndAnnotations(id, request.requirements, request.annotations);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> AddTeacherToCourse(
        CoursesService coursesService,
        AddTeacherToCourseModel request,
        Guid id)
    {
        var response = await coursesService.AddTeacherToCourse(id, request.userId);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> EditCourse(
        Guid id,
        EditCampusCourseModel request,
        CoursesService coursesService)
    {
        var response = await coursesService.Edit(
            id,
            request.name, 
            request.startYear, 
            request.maximumStudentsCount, 
            request.semester, 
            request.requirements, 
            request.annotations);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> ChangeStudentStatus(
        [FromBody] EditCourseStudentStatusModel request,
        CoursesService coursesService,
        [FromQuery] Guid studentId,
        [FromQuery] Guid id)
    {
        var response = await coursesService.ChangeStudentStatus(id, studentId, request.status);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> ChangeStudentMark(
        [FromBody] EditCourseStudentMarkModel request,
        CoursesService coursesService,
        [FromQuery] Guid studentId,
        [FromQuery] Guid id)
    {
        var response = await coursesService.ChangeStudentMark(id, studentId, request.markType, request.mark);
        
        return Results.Ok(response);
    }
}