namespace courses.Models.Entities;

public class UserEntity
{
    private UserEntity(Guid id, string fullName, DateTime birthDate, string email, string passwordHash)
    {
        Id = id;
        FullName = fullName;
        BirthDate = birthDate;
        Email = email;
        PasswordHash = passwordHash;
    }
    public Guid Id { get; set; }
    
    public string FullName { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    
    public List<RoleEntity> Roles { get; set; }

    public static UserEntity Create(Guid id, string fullName, DateTime birthDate, string email, string passwordHash)
    {
        return new UserEntity(id, fullName, birthDate, email, passwordHash);
    }
}