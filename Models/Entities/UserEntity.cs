namespace courses.Models.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    
    public string FullName { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    
    public List<RoleEntity> Roles { get; set; }
}