using courses.Models.enums;

namespace courses.Models.Entities;

public class CourseEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int StartYear { get; set; }
    
    public int MaximumStudentsCount { get; set; }
    
    public int RemainingStoltsCount { get; set; }
    
    public CourseStatuses Status { get; set; } 
    
    public Semesters Semester { get; set; }
    
    public Guid GroupId { get; set; }
    
    public GroupEntity? Group { get; set; }
        
    public List<TeacherEntity> Teachers { get; set; } = [];
    
    public List<StudentEntity> Students { get; set; } = [];
    
    public List<NotificationEntity> Notifications { get; set; } = [];
}