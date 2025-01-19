using courses.Models.Entities;
using courses.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface IUsersRepository
{
    Task Add(UserEntity user);
    
    Task<UserEntity> GetByEmail(string email);

    Task<UserEntity> GetById(Guid userId);

    Task<HashSet<Permission>> GetUserPermissions(Guid userId);

    Task Edit(Guid userId, string fullName, DateTime birthDate);
}

public class UsersRepository : IUsersRepository
{
    private readonly CoursesDbContext _context;

    public UsersRepository(CoursesDbContext context)
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
    
    public async Task<UserEntity> GetByEmail(string email)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email) ?? throw new Exception($"User with email {email} does not exist");

        return userEntity;
    }
    
    public async Task<UserEntity> GetById(Guid userId)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new Exception($"User with id {userId} does not exist");

        return userEntity;
    }

    public async Task<HashSet<Permission>> GetUserPermissions(Guid userId)
    {
        var roles = await _context.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .Where(u => u.Id == userId)
            .Select(u => u.Roles)
            .ToArrayAsync();
        
        return roles
            .SelectMany(r => r)
            .SelectMany(r => r.Permissions)
            .Select(p => (Permission)p.Id)
            .ToHashSet();
    }
}