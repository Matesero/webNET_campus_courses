namespace courses.Models.Entities;

public class BlackTokenEntity
{
    private BlackTokenEntity(Guid id, string token, DateTime expirationDate)
    {
        Id = id;
        Token = token;
        ExpirationDate = expirationDate;
    }
    
    public Guid Id { get; set; }
    
    public string Token { get; set; }
    
    public DateTime ExpirationDate { get; set; }

    public static BlackTokenEntity Create(string token)
    {
        return new BlackTokenEntity(Guid.NewGuid(), token, DateTime.UtcNow);
    }
}