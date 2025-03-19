namespace courses.Models.Entities;

public class PermissionEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public List<RoleEntity> Roles { get; set; } = [];
}