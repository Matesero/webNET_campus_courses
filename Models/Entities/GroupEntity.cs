namespace courses.Models.Entities;

public class GroupEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;

    public List<CourseEntity> Courses { get; set; } = [];
}