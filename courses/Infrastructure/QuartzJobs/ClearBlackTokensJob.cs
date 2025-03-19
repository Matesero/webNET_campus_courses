using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace courses.Infrastructure.QuartzJobs;

public class ClearBlackTokensJob: IJob
{
    private readonly CoursesDbContext _context;
    
    public ClearBlackTokensJob(CoursesDbContext context)
    {
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var today = DateTime.UtcNow;
        var day = TimeSpan.FromDays(1);

        var tokens = await _context.BlackTokens
            .AsNoTracking()
            .Where(token => token.ExpirationDate + day < today )
            .ToListAsync();
        
        if (tokens.Any())
        {
            _context.BlackTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }
    }
}