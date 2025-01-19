namespace courses.Models.Entities;

public class TeacherEntity
{
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }
    
    public Guid CourseId { get; set; }
    
    public CourseEntity Course { get; set; }
    
    public bool isMain { get; set; } = false;
}