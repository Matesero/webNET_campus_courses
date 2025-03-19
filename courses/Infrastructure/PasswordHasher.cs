namespace courses.Infrastructure;

public interface IPasswordHasher
{
    string Generate(string password);
    
    bool Verify(string hashedPassword, string providedPassword);
}

public class PasswordHasher: IPasswordHasher
{
    public string Generate(string password) => 
        BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    
    public bool Verify(string password, string hashedPassword) => 
        BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
}