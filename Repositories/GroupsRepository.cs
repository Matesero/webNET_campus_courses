using courses.Models.DTO;
using courses.Models.Entities;
using courses.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface IGroupsRepository
{
    Task<List<CampusGroupModel>> GetAll();
}

public class GroupsRepository : IGroupsRepository
{
    private readonly CoursesDbContext _context;

    public GroupsRepository(CoursesDbContext context)
    {
        _context = context;
    }

    public async Task Add(UserEntity userEntity)
    {
        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync();
    }
    
    public async Task Edit(Guid userId, string fullName, DateTime birthDate)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new Exception($"User with id {userId} does not exist");
        
        user.FullName = fullName;
        user.BirthDate = birthDate;
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<CampusGroupModel>> GetAll()
    {
        var groups = await _context.Groups
            .AsNoTracking()
            .Select(g => new CampusGroupModel
            {
                id = g.Id,
                name = g.Name
            })
            .ToListAsync();
        
        return groups;
    }
}