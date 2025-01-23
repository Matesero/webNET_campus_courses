using System.ComponentModel.DataAnnotations;
using courses.Extensions;
using courses.Models.DTO;
using courses.Models.enums;
using courses.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace courses.Endpoints;

public static class CoursesEndpoints
{
    public static IEndpointRouteBuilder MapCoursesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var courses = endpoints.MapGroup("/courses").RequireAuthorization();
        
        courses.MapGet("/{id}/details", GetCourseDetailedInfo);
        
        endpoints.MapPost("/groups/{groupId}", CreateCourse)
            .RequirePermissions(Permission.Create);

        courses.MapPost("/{id}/status", EditCoursesStatus);

        courses.MapPut("/{id}/requirements-and-annotations", EditCoursesRequirementsAndAnnotations);
        
        courses.MapDelete("/{id}", DeleteCourse)
            .RequirePermissions(Permission.Delete);

        courses.MapPost("/{id}/notification", CreateNotification);
        
        courses.MapPut("/{id}", EditCourse)
            .RequirePermissions(Permission.Update);

        courses.MapPost("/{id}/sign-up", SignUpCourse);

        courses.MapPost("/{id}/teacher", AddTeacherToCourse);
        
        courses.MapGet("/my", GetMyCourses);
        
        courses.MapGet("/teaching", GetTeachingCourses);
        
        courses.MapGet("/list", GetFilteredCourses);

        courses.MapPost("/{id}/student-status/{studentId}", ChangeStudentStatus);

        courses.MapPost("/{id}/marks/{studentId}", ChangeStudentMark);
        
        return endpoints;
    }

    [Authorize]
    private static async Task<IResult> CreateCourse(
        Guid groupId,
        CreateCampusCourseModel request,
        IValidator<CreateCampusCourseModel> validator,
        CoursesService coursesService)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
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
        await coursesService.Delete(id);
        
        return Results.Ok();
    }
    
    [Authorize]
    private static async Task<IResult> SignUpCourse(
        Guid courseId,
        CoursesService coursesService,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        
        await coursesService.SignUp(userId, courseId);
        
        return Results.Ok();
    }

    [Authorize]
    private static async Task<IResult> GetMyCourses(
        CoursesService coursesService,
        HttpContext context)
    {
        var userId = context.User.GetUserId();

        var response = await coursesService.GetMyCourses(userId);

        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> GetTeachingCourses(
        CoursesService coursesService,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        
        var response = await coursesService.GetTeachingCourses(userId);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> CreateNotification(
        Guid id, 
        HttpContext context,
        CampusCourseNotificationModel request,
        IValidator<CampusCourseNotificationModel> validator,
        CoursesService coursesService)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var userId = context.User.GetUserId();
        
        var response = await coursesService.CreateNotification(
            id, 
            userId, 
            request.text, 
            request.isImportant);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> GetCourseDetailedInfo(
        CoursesService coursesService,
        HttpContext context,
        Guid id)
    {
        var userId = context.User.GetUserId();
        
        var response = await coursesService.GetDetailedInfo(id, userId);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> GetFilteredCourses(
        CoursesService coursesService,
        IValidator<CampusCourseFilterModel> validator,
        [FromQuery] SortList? sort,
        [FromQuery] string? search,
        [FromQuery] bool? hasPlacesAndOpen,
        [FromQuery] Semesters? semester,
        [FromQuery, Range(1, int.MaxValue)] int page,
        [FromQuery, Range(1, int.MaxValue)] int pageSize)
    {
        var request = new CampusCourseFilterModel
        {
            sort = sort ?? null,
            search = search ?? null,
            hasPlacesAndOpen = hasPlacesAndOpen ?? null,
            semester = semester ?? null,
            page = page,
            pageSize = pageSize
        };
        
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var response = await coursesService.GetFilteredCourses(
            sort, 
            search, 
            hasPlacesAndOpen, 
            semester, 
            page, 
            pageSize);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> EditCoursesStatus(
        CoursesService coursesService,
        EditCourseStatusModel request,
        IValidator<EditCourseStatusModel> validator,
        HttpContext context,
        Guid id)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var userId = context.User.GetUserId();
        
        var response = await coursesService.EditCoursesStatus(id, userId, request.status);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> EditCoursesRequirementsAndAnnotations(
        CoursesService coursesService,
        EditCampusCourseRequirementsAndAnnotationsModel request,
        IValidator<EditCampusCourseRequirementsAndAnnotationsModel> validator,
        HttpContext context,
        Guid id)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var userId = context.User.GetUserId();
        
        var response = await coursesService.EditCoursesRequirementsAndAnnotations(
            id, 
            userId,
            request.requirements, 
            request.annotations);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> AddTeacherToCourse(
        CoursesService coursesService,
        AddTeacherToCourseModel request,
        HttpContext context,
        Guid id)
    {
        var userId = context.User.GetUserId();
        
        var response = await coursesService.AddTeacherToCourse(id, userId, request.userId);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> EditCourse(
        Guid id,
        EditCampusCourseModel request,
        IValidator<EditCampusCourseModel> validator,
        CoursesService coursesService)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
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
        IValidator<EditCourseStudentStatusModel> validator,
        CoursesService coursesService,
        [FromQuery] Guid studentId,
        [FromQuery] Guid id,
        HttpContext context)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var userId = context.User.GetUserId();
        
        var response = await coursesService.ChangeStudentStatus(
            id, 
            userId,
            studentId, 
            request.status);
        
        return Results.Ok(response);
    }

    [Authorize]
    private static async Task<IResult> ChangeStudentMark(
        [FromBody] EditCourseStudentMarkModel request,
        IValidator<EditCourseStudentMarkModel> validator,
        CoursesService coursesService,
        [FromQuery] Guid studentId,
        [FromQuery] Guid id,
        HttpContext context)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var userId = context.User.GetUserId();
        
        var response = await coursesService.ChangeStudentMark(
            id, 
            userId,
            studentId, 
            request.markType, 
            request.mark);
        
        return Results.Ok(response);
    }
}