namespace courses.Models.Entities;

public class NotificationEntity
{
    private NotificationEntity(Guid courseId, string text, bool isImportant)
    {
        CourseId = courseId;
        Text = text;
        IsImportant = isImportant;
    }
    
    public Guid CourseId { get; set; }
    
    public CourseEntity Course { get; set; }
    
    public string Text { get; set; }
    
    public bool IsImportant { get; set; }
    
    public static NotificationEntity Create(Guid courseId, string text, bool isImportant)
    {
        return new NotificationEntity(courseId, text, isImportant);
    }
}