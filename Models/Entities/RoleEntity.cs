namespace courses.Models.Entities;

public class RoleEntity
{
    public Guid Id { get; set; }
    
    public List<UserEntity> Users { get; set; }
}