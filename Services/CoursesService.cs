using courses.Middleware;
using courses.Models.DTO;
using courses.Models.Entities;
using courses.Models.enums;
using courses.Repositories;

namespace courses.Services;

public interface ICoursesService
{
    Task<List<CampusCoursePreviewModel>> Create(
        Guid groupId,
        string name,
        int startYear,
        int maximumStudentsCount,
        string semester,
        string requirements,
        string annotations,
        Guid mainTeacherId);

    Task Delete(Guid id);
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

    public async Task<List<CampusCoursePreviewModel>> Create(
        Guid groupId, 
        string name, 
        int startYear,
        int maximumStudentsCount, 
        string semester, 
        string requirements, 
        string annotations, 
        Guid mainTeacherId)
    {
        await _groupsRepository.CheckExistence(groupId);
        await _usersRepository.CheckExistence(mainTeacherId);
        
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

        var mainTeacher = TeacherEntity.Create(
            mainTeacherId, 
            courseId, 
            true, 
            groupId);

        await _coursesRepository.Add(course);
        await _teachersRepository.Add(mainTeacher);
        
        var courses = await _groupsRepository.GetCourses(groupId);

        return courses.Select(courseEntity => ConvertEntityToPreviewModel(courseEntity)).ToList();
    }
    
    public async Task Delete(Guid id)
    {
        await _coursesRepository.Delete(id);
    }

    public async Task SignUp(string id, Guid courseId)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            throw new Exception(); // обработать
        }
        
        var courseEntity = await _coursesRepository.GetByIdWithStudentsAndTeachers(courseId);

        if (courseEntity.Status != Enum.GetName(CourseStatuses.OpenForAssigning))
        {
            throw new KeyNotFoundException($"Course requires at least 1 slot"); // Обработать
        }

        var remainingSlots = courseEntity.MaximumStudentsCount - 
                             courseEntity.Students.Count(student => student.Status == "Accepted");
        
        if (remainingSlots <= 0 )
        {
            throw new KeyNotFoundException($"Course requires at least 2 slot"); // Обработать
        }
        
        if (courseEntity.Students.Any(student => student.UserId.ToString() == id))
        {
            throw new KeyNotFoundException($"Course requires at least 3 slot");
        }
        
        if (courseEntity.Teachers.Any(teacher => teacher.UserId.ToString() == id))
        {
            throw new KeyNotFoundException($"Course requires at least 4 slot");
        }

        var studentEntity = StudentEntity.Create(userId, courseId);
        
        await _studentsRepository.Add(studentEntity);
    }

    public async Task<List<CampusCoursePreviewModel>> GetMyCourses(string id)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            throw new Exception();
        }
        
        var courses = await _coursesRepository.GetByStudentId(userId);
        
        return courses.Select(course => ConvertEntityToPreviewModel(course)).ToList();
    }
    
    public async Task<List<CampusCoursePreviewModel>> GetTeachingCourses(string id)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            throw new Exception(); // обработать
        }
        
        var courses = await _coursesRepository.GetByTeacherId(userId);
        
        return courses.Select(course => ConvertEntityToPreviewModel(course)).ToList();
    }

    public async Task<CampusCourseDetailsModel> CreateNotification(Guid courseId, string text, bool isImportant)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(courseId);

        var notificationEntity = NotificationEntity.Create(courseId, text, isImportant);
        
        await _notificationRepository.Add(notificationEntity);
        
        courseEntity.Notifications.Add(notificationEntity);
        
        return ConvertEntityToDetailedModel(courseEntity);
    }

    public async Task<CampusCourseDetailsModel> GetDetailedInfo(Guid id)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(id);

        return ConvertEntityToDetailedModel(courseEntity);
    }

    public async Task<List<CampusCoursePreviewModel>> GetFilteredCourses(
        SortList? sort,
        string? search,
        bool? hasPlacesAndOpen,
        Semesters? semester,
        int page,
        int pageSize)
    {
        var courses = await _coursesRepository
            .GetByFiltersAndPagination(
            sort, 
            search, 
            hasPlacesAndOpen, 
            semester, 
            page, 
            pageSize);
        
        return courses.Select(course => ConvertEntityToPreviewModel(course)).ToList();
    }

    public async Task<CampusCourseDetailsModel> EditCoursesStatus(Guid id, string statusString)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(id);
        
        var status = (CourseStatuses)Enum.Parse(typeof(CourseStatuses), statusString);
        
        if (!Enum.TryParse<CourseStatuses>(courseEntity.Status, out var dbStatus))
        {
            courseEntity.Status = statusString;
        }
         else if ((int)dbStatus < (int)status)
        {
            courseEntity.Status = statusString;
        }
         else
         {
             throw new Exception(); // обработать
         }
        
        await _coursesRepository.Update(courseEntity);
        
        return ConvertEntityToDetailedModel(courseEntity);
    }
    
    public async Task<CampusCourseDetailsModel> EditCoursesRequirementsAndAnnotations(
        Guid id,
        string requirements, 
        string annotations)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(id);
        
        courseEntity.Requirements = requirements;
        courseEntity.Annotations = annotations;
        
        await _coursesRepository.Update(courseEntity);
        
        return ConvertEntityToDetailedModel(courseEntity);
    }
    
    public async Task<CampusCourseDetailsModel> AddTeacherToCourse(Guid courseId, Guid teacherId)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(courseId);

        if (courseEntity.Students.Any(student => student.UserId == teacherId) ||
            courseEntity.Teachers.Any(teacher => teacher.UserId == teacherId))
        {
            throw new Exception(); // обработать
        }
        
        var user = await _usersRepository.GetById(teacherId);
        
        var teacher = TeacherEntity.Create(
            teacherId,
            courseId,
            false
        );
        
        await _teachersRepository.Add(teacher);
        
        courseEntity.Teachers.Add(teacher);
        teacher.User = user;
        
        return ConvertEntityToDetailedModel(courseEntity);
    }

    private CampusCourseDetailsModel ConvertEntityToDetailedModel(CourseEntity courseEntity)
    {
        var students = courseEntity.Students
            .Select(student => new CampusCourseStudentModel
        {
            id = student.UserId,
            name = student.User.FullName,
            email = student.User.Email,
            status = student.Status,
            midtermResult = student.MidtermResult, 
            finalResult = student.FinalResult,
        }).ToList();
        
        var teachers = courseEntity.Teachers
            .Select(teacher => new CampusCourseTeacherModel
        {
            name = teacher.User.FullName,
            email = teacher.User.Email,
            isMain = teacher.IsMain
        }).ToList();
        
        var notifications = courseEntity.Notifications
            .Select(notification => new CampusCourseNotificationModel
        {
            text = notification.Text,
            isImportant = notification.IsImportant,
        }).ToList();
        
        return new CampusCourseDetailsModel
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

    private CampusCoursePreviewModel ConvertEntityToPreviewModel(CourseEntity courseEntity)
    {
        return new CampusCoursePreviewModel
        {
            id = courseEntity.Id,
            name = courseEntity.Name,
            startYear = courseEntity.StartYear,
            maximumStudentsCount = courseEntity.MaximumStudentsCount,
            remainingSlotsCount = Math.Max(0, courseEntity.MaximumStudentsCount - 
                                              courseEntity.Students.Count(student => student.Status == "Accepted")),
            semester = courseEntity.Semester,
            status = courseEntity.Status
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
        
        courseEntity.Name = name;
        courseEntity.StartYear = startYear;

        if (maximumStudentsCount < courseEntity.Students.Count(student => student.Status == "Accepted"))
        {
            throw new Exception(); // обработать 
        }
        courseEntity.MaximumStudentsCount = maximumStudentsCount;
        
        courseEntity.Semester = semester;
        courseEntity.Requirements = requirements;
        courseEntity.Annotations = annotations;
        
        await _coursesRepository.Update(courseEntity);
        return ConvertEntityToDetailedModel(courseEntity); 
    }

    public async Task<CampusCourseDetailsModel> ChangeStudentStatus(Guid courseId, Guid studentId, string status)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(courseId);
        
        var student = courseEntity.Students.FirstOrDefault(student => student.UserId == studentId);

        if (student is null)
        {
            throw new NotFoundException(studentId.ToString(),"Student", "ID");
        }

        if (student.Status != Enum.GetName(StudentStatuses.InQueue))
        {
            throw new Exception(); // обработать
        }
        
        student.Status = status;
        
        courseEntity.Students = courseEntity.Students
            .Where(s => s.UserId != studentId)
            .Append(student)
            .ToList();
        
        await _studentsRepository.Update(student);
         
        return ConvertEntityToDetailedModel(courseEntity);
    }
    
    public async Task<CampusCourseDetailsModel> ChangeStudentMark(
        Guid courseId, 
        Guid studentId, 
        string markType, 
        string mark)
    {
        var courseEntity = await _coursesRepository.GetDetailedInfoById(courseId);
        
        var student = courseEntity.Students.FirstOrDefault(student => student.UserId == studentId);

        if (student is null)
        {
            throw new KeyNotFoundException($"Student with id {studentId} not found");
        }
        
        if (markType == Enum.GetName(MarkType.Midterm))
        {
            student.MidtermResult = mark;
        }
        else
        {
            student.FinalResult = mark;
        }
        
        courseEntity.Students = courseEntity.Students
            .Where(s => s.UserId != studentId)
            .Append(student)
            .ToList();
        
        await _studentsRepository.Update(student);
         
        return ConvertEntityToDetailedModel(courseEntity);
    }
}