namespace courses.Models.Entities;

public class RoleEntity
{
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }
    
    public string Name { get; set; }
}