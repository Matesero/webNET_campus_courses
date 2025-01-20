using courses.Models.enums;

namespace courses.Models.Entities;

public class CourseEntity
{
    private CourseEntity(
        Guid id, 
        string name, 
        int startYear, 
        int maximumStudentsCount,
        int remainingSlotsCount,
        string requirements,
        string annotation,
        CourseStatuses status, 
        Semesters semester, 
        Guid groupId)
    {
        Id = id;
        Name = name;
        StartYear = startYear;
        MaximumStudentsCount = maximumStudentsCount;
        RemainingSlotsCount = remainingSlotsCount;
        Requirements = requirements;
        Annotation = annotation;
        Status = status;
        Semester = semester;
        GroupId = groupId;
    }
    
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int StartYear { get; set; }
    
    public int MaximumStudentsCount { get; set; }

    public int RemainingSlotsCount { get; set; }
    
    public string Requirements { get; set; }
    
    public string Annotation { get; set; }
    
    public CourseStatuses Status { get; set; } 
    
    public Semesters Semester { get; set; }
    
    public Guid GroupId { get; set; }
    
    public GroupEntity? Group { get; set; }
        
    public List<TeacherEntity> Teachers { get; set; } = [];
    
    public List<StudentEntity> Students { get; set; } = [];
    
    public List<NotificationEntity> Notifications { get; set; } = [];
    
    public static CourseEntity Create(
        Guid id, 
        string name, 
        int startYear, 
        int maximumStudentsCount, 
        string requirements,
        string annotation,
        Semesters semester, 
        Guid groupId)
    {
        return new CourseEntity(
            id, 
            name, 
            startYear, 
            maximumStudentsCount, 
            maximumStudentsCount,
            requirements, 
            annotation, 
            CourseStatuses.Created, 
            semester, 
            groupId);
    }
}