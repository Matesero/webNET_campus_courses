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
        Semesters semester,
        string requirements,
        string annotations,
        Guid mainTeacherId);
    
    
}

public class CoursesService : ICoursesService
{
    private readonly ICoursesRepository _coursesRepository;
    private readonly IGroupsRepository _groupsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ITeachersRepository _teachersRepository;

    public CoursesService(
        ICoursesRepository coursesRepository, 
        IGroupsRepository groupsRepository,
        IUsersRepository usersRepository,
        ITeachersRepository teachersRepository)
    {
        _coursesRepository = coursesRepository;
        _groupsRepository = groupsRepository;
        _usersRepository = usersRepository;
        _teachersRepository = teachersRepository;
    }

    public async Task<CampusCoursePreviewModel> Create(
        Guid groupId, 
        string name, 
        int startYear,
        int maximumStudentsCount, 
        Semesters semester, 
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
            groupId);

        var mainTeacher = TeacherEntity.Create(mainTeacherId, courseId, true);

        await _coursesRepository.Add(course);
        
        await _teachersRepository.Add(mainTeacher);
        
        return new CampusCoursePreviewModel
        {
            id = courseId,
            name = name,
            startYear = startYear,
            maximumStudentsCount = maximumStudentsCount,
            semester = semester,
            status = CourseStatuses.Created
        };
    }

    
}