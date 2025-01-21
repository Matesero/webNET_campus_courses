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
        string status, 
        string semester, 
        Guid mainTeacherId,
        Guid groupId,
        DateTime createdDate)
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
        MainTeacherId = mainTeacherId;
        GroupId = groupId;
        CreatedDate = createdDate;
    }
    
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int StartYear { get; set; }
    
    public int MaximumStudentsCount { get; set; }

    public int RemainingSlotsCount { get; set; }
    
    public string Requirements { get; set; }
    
    public string Annotation { get; set; }
    
    public string Status { get; set; } 
    
    public string Semester { get; set; }
    
    public Guid MainTeacherId { get; set; }
    
    public Guid GroupId { get; set; }
    
    public GroupEntity? Group { get; set; }
    
    public DateTime? NotificationDate { get; set; }
    
    public DateTime CreatedDate { get; set; }
        
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
        string semester, 
        Guid mainTeacherId,
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
            Enum.GetName(typeof(CourseStatuses), CourseStatuses.Created), 
            semester, 
            mainTeacherId,
            groupId,
            DateTime.Now.ToUniversalTime());
        ;
    }
}