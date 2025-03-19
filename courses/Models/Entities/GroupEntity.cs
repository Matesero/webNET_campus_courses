namespace courses.Models.Entities;

public class GroupEntity
{
    private GroupEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public List<CourseEntity> Courses { get; set; } = [];
    
    public static GroupEntity Create(Guid id, string name)
    {
        return new GroupEntity(id, name);
    }
}