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
        
        courses.MapPost("/{id}/status", EditCoursesStatus)
            .RequirePermissions(Permission.Update);
        
        courses.MapPost("/{id}/requirements-and-annotations", EditCoursesRequirementsAndAnnotations)
            .RequirePermissions(Permission.Update);
        
        courses.MapDelete("/{id}", DeleteCourse)
            .RequirePermissions(Permission.Delete);
        
        courses.MapPost("/{id}/notification", CreateNotification)
            .RequirePermissions(Permission.Create);
        
        courses.MapPut("/{id}", EditCourse)
            .RequirePermissions(Permission.Update);

        courses.MapPost("/{id}/sign-up", SignUpCourse);
        
        courses.MapPost("/{id}/teacher", AddTeacherToCourse)
            .RequirePermissions(Permission.Update);
        
        courses.MapGet("/my", GetMyCourses);
        
        courses.MapGet("/teaching", GetTeachingCourses);
        
        courses.MapGet("/list", GetFilteredCourses);
        
        courses.MapPost("/{id}/student-status/{studentId}", ChangeStudentStatus)
            .RequirePermissions(Permission.Update);
        
        courses.MapPost("/{id}/marks/{studentId}", ChangeStudentMark)
            .RequirePermissions(Permission.Update);
        
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
        IValidator<CampusCourseNotificationModel> validator,
        CoursesService coursesService)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var response = await coursesService.CreateNotification(id, request.text, request.isImportant);
        
        return Results.Ok(response);
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
            sort = sort,
            search = search,
            hasPlacesAndOpen = hasPlacesAndOpen,
            semester = semester,
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
        Guid id)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var response = await coursesService.EditCoursesStatus(id, request.status);
        
        return Results.Ok(response);
    }
    
    [Authorize]
    private static async Task<IResult> EditCoursesRequirementsAndAnnotations(
        CoursesService coursesService,
        EditCampusCourseRequirementsAndAnnotationsModel request,
        IValidator<EditCampusCourseRequirementsAndAnnotationsModel> validator,
        Guid id)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var response = await coursesService.EditCoursesRequirementsAndAnnotations(
            id, 
            request.requirements, 
            request.annotations);
        
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
        [FromQuery] Guid id)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var response = await coursesService.ChangeStudentStatus(
            id, 
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
        [FromQuery] Guid id)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var response = await coursesService.ChangeStudentMark(
            id, 
            studentId, 
            request.markType, 
            request.mark);
        
        return Results.Ok(response);
    }
}