namespace courses.Models.Entities;

public class TeacherEntity
{
    private TeacherEntity(Guid userId, Guid courseId, bool isMain)
    {
        UserId = userId;
        CourseId = courseId;
        IsMain = isMain;
    }
    
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }
    
    public Guid CourseId { get; set; }
    
    public CourseEntity Course { get; set; }
    
    public bool IsMain { get; set; }
    
    public static TeacherEntity Create(Guid userId, Guid courseId, bool isMain)
    {
        return new TeacherEntity(userId, courseId, isMain);
    }
}