namespace courses.Models.Entities;

public class NotificationEntity
{
    public Guid Id { get; set; }
    
    public Guid CourseId { get; set; }
    
    public CourseEntity Course { get; set; }
    
    public string Text { get; set; } = string.Empty;
    
    public bool IsImportant { get; set; } = false;
}