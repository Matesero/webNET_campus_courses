using courses.Infrastructure;
using courses.Models.DTO;
using courses.Models.Entities;
using courses.Models.enums;
using courses.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace courses.Services;

public interface ICoursesService
{
    Task<CampusCoursePreviewModel> Create(
        Guid groupId,
        string name,
        int startYear,
        int maximumStudentsCount,
        string semester,
        string requirements,
        string annotations,
        Guid mainTeacherId);

    Task<CampusCoursePreviewModel> Delete(Guid id);
}

public class CoursesService : ICoursesService
{
    private readonly ICoursesRepository _coursesRepository;
    private readonly IGroupsRepository _groupsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ITeachersRepository _teachersRepository;
    private readonly IStudentsRepository _studentsRepository;
    private readonly INotificationsRepository _notificationRepository;


    public CoursesService(
        ICoursesRepository coursesRepository, 
        IGroupsRepository groupsRepository,
        IUsersRepository usersRepository,
        ITeachersRepository teachersRepository,
        IStudentsRepository studentsRepository,
        INotificationsRepository notificationRepository)
    {
        _coursesRepository = coursesRepository;
        _groupsRepository = groupsRepository;
        _usersRepository = usersRepository;
        _teachersRepository = teachersRepository;
        _studentsRepository = studentsRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<CampusCoursePreviewModel> Create(
        Guid groupId, 
        string name, 
        int startYear,
        int maximumStudentsCount, 
        string semester, 
        string requirements, 
        string annotations, 
        Guid mainTeacherId)
    {
        var group = await _groupsRepository.GetById(groupId);

        if (group is null)
        {
            throw new KeyNotFoundException($"Group with id {groupId} not found"); // обработать
        }
        
        var teacher = await _usersRepository.GetById(mainTeacherId);
        
        if (teacher is null)
        {
            throw new KeyNotFoundException($"User with id {mainTeacherId} not found"); // обработать
        }
        
        var courseId = Guid.NewGuid();
        
        var course = CourseEntity.Create(
            courseId, 
            name, 
            startYear, 
            maximumStudentsCount,
            requirements, 
            annotations, 
            semester, 
            mainTeacherId,
            groupId);

        var mainTeacher = TeacherEntity.Create(mainTeacherId, courseId, true);

        await _coursesRepository.Add(course);
        
        await _teachersRepository.Add(mainTeacher);
        
        return new CampusCoursePreviewModel
        {
            id = courseId,
            name = course.Name,
            startYear = course.StartYear,
            maximumStudentsCount = course.MaximumStudentsCount,
            semester = course.Semester,
            status = course.Status
        };
    }
    
    public async Task<CampusCoursePreviewModel> Delete(Guid id)
    {
        var course = await _coursesRepository.GetById(id);

        if (course is null)
        {
            throw new Exception(); // обработать
        }

        await _coursesRepository.Delete(course);
        
        return new CampusCoursePreviewModel
        {
            id = course.Id,
            name = course.Name,
            maximumStudentsCount = course.MaximumStudentsCount,
            remainingSlotsCount = course.RemainingSlotsCount,
            status = course.Status,
            semester = course.Semester
        };
    }

    public async Task SignUp(string id, Guid courseId)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            throw new Exception(); // обработать
        }
        
        var course = await _coursesRepository.GetById(courseId);

        if (course is null)
        {
            throw new KeyNotFoundException($"Course with id {courseId} not found"); // Обработать
        }

        if (course.RemainingSlotsCount < 1)
        {
            throw new KeyNotFoundException($"Course requires at least 1 slot"); // Обработать
        }

        var student = StudentEntity.Create(userId, courseId);
        
        await _studentsRepository.Add(student);
    }

    public async Task<List<CampusCoursePreviewModel>> GetMyCourses(string id)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            throw new Exception();
        }
        
        var courses = await _coursesRepository.GetByStudentId(userId);
        
        return courses.Select(course => new CampusCoursePreviewModel
        {
            id = course.Id,
            name = course.Name,
            startYear = course.StartYear,
            maximumStudentsCount = course.MaximumStudentsCount,
            remainingSlotsCount = course.RemainingSlotsCount,
            status = course.Status,
            semester = course.Semester
        }).ToList();
    }
    
    public async Task<List<CampusCoursePreviewModel>> GetTeachingCourses(string id)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            throw new Exception();
        }
        
        var courses = await _coursesRepository.GetByTeacherId(userId);
        
        return courses.Select(course => new CampusCoursePreviewModel
        {
            id = course.Id,
            name = course.Name,
            startYear = course.StartYear,
            maximumStudentsCount = course.MaximumStudentsCount,
            remainingSlotsCount = course.RemainingSlotsCount,
            status = course.Status,
            semester = course.Semester
        }).ToList();
    }

    public async Task CreateNotification(Guid courseId, string text, bool isImportant)
    {
        var course = await _coursesRepository.GetById(courseId);

        if (course is null)
        {
            throw new KeyNotFoundException($"Course with id {courseId} not found");
        }
        
        var notification = NotificationEntity.Create(courseId, text, isImportant);
        
        await _notificationRepository.Add(notification);
    }
    
    // public async Task<CampusCoursePreviewModel> Edit(
    //     Guid id, 
    //     string name, 
    //     int startYear,
    //     int maximumStudentsCount, 
    //     Semesters semester, 
    //     string requirements, 
    //     string annotations, 
    //     Guid mainTeacherId)
    // {
    //     var course = await _coursesRepository.GetById(id);
    //
    //     if (course is null)
    //     {
    //         throw new KeyNotFoundException($"Course with id {id} not found");
    //     }
    //
    //     if (course.MainTeacherId != mainTeacherId)
    //     {
    //         var teacher = await _usersRepository.GetById(mainTeacherId);
    //         
    //         if (teacher is null)
    //         {
    //             throw new KeyNotFoundException($"User with id {mainTeacherId} not found"); // обработать
    //         }
    //
    //     }
    //     
    //
    //     await _coursesRepository.Add(course);
    //     
    //     await _teachersRepository.Add(mainTeacher);
    //     
    //     return new CampusCoursePreviewModel
    //     {
    //         id = courseId,
    //         name = name,
    //         startYear = startYear,
    //         maximumStudentsCount = maximumStudentsCount,
    //         semester = semester,
    //         status = CourseStatuses.Created
    //     };
    // }
}