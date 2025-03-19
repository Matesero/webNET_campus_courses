using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;

public interface IBlackTokensRepository
{
    Task Add(string token);
    Task<BlackTokenEntity> Find(string token);
}

public class BlackTokensRepository : IBlackTokensRepository
{
    private readonly CoursesDbContext _context;

    public BlackTokensRepository(CoursesDbContext context)
    {
        _context = context;
    }

    public async Task Add(string token)
    {
        var tokenEntity = BlackTokenEntity.Create(token);
        
        await _context.BlackTokens.AddAsync(tokenEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<BlackTokenEntity> Find(string token)
    {
        var tokenEntity = await _context.BlackTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == token);
        
        return tokenEntity;
    }
}