﻿using courses.Models.DTO;
using courses.Models.Entities;
using courses.Models.enums;
using courses.Repositories;
using Microsoft.Extensions.Logging.Console;

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

        var mainTeacher = TeacherEntity.Create(mainTeacherId, courseId, true, groupId);

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
        var course = await _coursesRepository.GetByIdWithStudents(id);

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
            remainingSlotsCount = Math.Max(0, course.MaximumStudentsCount - course.Students.Count(student => student.Status == "Accepted")),
            semester = course.Semester,
            status = course.Status
        };
    }

    public async Task SignUp(string id, Guid courseId)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            throw new Exception(); // обработать
        }
        
        var course = await _coursesRepository.GetByIdWithStudents(courseId);

        if (course is null)
        {
            throw new KeyNotFoundException($"Course with id {courseId} not found"); // Обработать
        }

        if (course.MaximumStudentsCount - course.Students.Count <= 0)
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
                remainingSlotsCount = Math.Max(0, course.MaximumStudentsCount - course.Students.Count(student => student.Status == "Accepted")),
                semester = course.Semester,
                status = course.Status
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
                remainingSlotsCount = Math.Max(0, course.MaximumStudentsCount - course.Students.Count(student => student.Status == "Accepted")),
                semester = course.Semester,
                status = course.Status
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

    public async Task<CampusCourseDetailsModel> GetDetailedInfo(Guid id)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(id);
        
        var course = ConvertEntityToModel(courseEntity);

        return course;
    }

    public async Task<List<CampusCoursePreviewModel>> GetFilteredCourses(
        SortList? sort,
        string? search,
        bool? hasPlacesAndOpen,
        Semesters? semester,
        int page,
        int pageSize)
    {
        var courses = await _coursesRepository.GetByFiltersAndPagination(sort, search, hasPlacesAndOpen, semester, page, pageSize);
        
        return courses.Select(course => new CampusCoursePreviewModel{
                id = course.Id,
                name = course.Name,
                startYear = course.StartYear,
                maximumStudentsCount = course.MaximumStudentsCount,
                remainingSlotsCount = Math.Max(0, course.MaximumStudentsCount - course.Students.Count),
                semester = course.Semester,
                status = course.Status
        }).ToList();
    }

    public async Task<CampusCourseDetailsModel> EditCoursesStatus(Guid id, string status)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(id);

        if (courseEntity is null)
        {
            throw new KeyNotFoundException($"Course with id {id} not found");
        }
        
        if (Enum.TryParse<CourseStatuses>(status, out var requestStatus))
        {
            if (!Enum.TryParse<CourseStatuses>(courseEntity.Status, out var dbStatus))
            {
                courseEntity.Status = requestStatus.ToString();
            }
            else
            {
                if ((int)dbStatus < (int)requestStatus)
                {
                    courseEntity.Status = requestStatus.ToString();
                }
                else
                {
                    throw new Exception(); // обработать
                }
            }
        }
        else
        {
            throw new Exception(); // обработать
        }
        
        await _coursesRepository.Update(courseEntity);
        
        return ConvertEntityToModel(courseEntity);
    }
    
    public async Task<CampusCourseDetailsModel> EditCoursesRequirementsAndAnnotations(Guid id, string requirements, string annotations)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(id);

        if (courseEntity is null)
        {
            throw new KeyNotFoundException($"Course with id {id} not found");
        }
        
        courseEntity.Requirements = requirements;
        courseEntity.Annotations = annotations;
        
        await _coursesRepository.Update(courseEntity);
        
        return ConvertEntityToModel(courseEntity);
    }
    
    public async Task<CampusCourseDetailsModel> AddTeacherToCourse(Guid courseId, Guid teacherId)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(courseId);
        
        if (courseEntity is null)
        {
            throw new KeyNotFoundException($"Course with id {courseId} not found");
        }

        if (courseEntity.Students.Any(student => student.UserId == teacherId) ||
            courseEntity.Teachers.Any(teacher => teacher.UserId == teacherId))
        {
            throw new Exception(); // обработать
        }
        
        var user = await _usersRepository.GetById(teacherId);

        if (user is null)
        {
            throw new KeyNotFoundException($"User with id {teacherId} not found");
        }

        var teacher = TeacherEntity.Create(
            teacherId,
            courseId,
            false
        );
        
        await _teachersRepository.Add(teacher);
        
        courseEntity.Teachers.Add(teacher);
        teacher.User = user;
        
        return ConvertEntityToModel(courseEntity);
    }

    private CampusCourseDetailsModel ConvertEntityToModel(CourseEntity courseEntity)
    {
        var students = courseEntity.Students.Select(student => new CampusCourseStudentModel
        {
            id = student.UserId,
            name = student.User.FullName,
            email = student.User.Email,
            status = student.Status,
            midtermResult = student.MidtermResult, 
            finalResult = student.FinalResult,
        }).ToList();
        
        var teachers = courseEntity.Teachers.Select(teacher => new CampusCourseTeacherModel
        {
            name = teacher.User.FullName,
            email = teacher.User.Email,
            isMain = teacher.IsMain
        }).ToList();
        
        var notifications = courseEntity.Notifications.Select(notification => new CampusCourseNotificationModel
        {
            text = notification.Text,
            isImportant = notification.IsImportant,
        }).ToList();
        
        return  new CampusCourseDetailsModel
        {
            id = courseEntity.Id,
            name = courseEntity.Name,
            startYear = courseEntity.StartYear,
            maximumStudentsCount = courseEntity.MaximumStudentsCount,
            studentsEnrolledCount = students.Count(student => student.status == "Accepted"),
            studentsInQueueCount = students.Count(student => student.status == "InQueue"),
            requirements = courseEntity.Requirements,
            annotations = courseEntity.Annotations,
            status = courseEntity.Status,
            semester = courseEntity.Semester,
            students = students,
            teachers = teachers,
            notifications = notifications,
        };
    }
    
    public async Task<CampusCourseDetailsModel> Edit(
        Guid id, 
        string name, 
        int startYear,
        int maximumStudentsCount, 
        string semester, 
        string requirements, 
        string annotations)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(id);
    
        if (courseEntity is null)
        {
            throw new KeyNotFoundException($"Course with id {id} not found");
        }
        
        courseEntity.Name = name;
        courseEntity.StartYear = startYear;

        if (courseEntity.MaximumStudentsCount < courseEntity.Students.Count(student => student.Status == "Accepted"))
        {
            throw new Exception(); // обработать 
        }
        courseEntity.MaximumStudentsCount = maximumStudentsCount;
        
        if (Enum.TryParse<Semesters>(semester, out var requestSemester))
        {
            courseEntity.Semester = semester;
        }
        else
        {
            throw new Exception(); // обработать
        }
        courseEntity.Requirements = requirements;
        courseEntity.Annotations = annotations;
        
        await _coursesRepository.Update(courseEntity);
        return ConvertEntityToModel(courseEntity); 
    }
}