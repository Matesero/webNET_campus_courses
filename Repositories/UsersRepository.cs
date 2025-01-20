using courses.Models.Entities;
using courses.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface IUsersRepository
{
    Task Add(UserEntity user);
    
    Task<UserEntity> GetByEmail(string email);

    Task<UserEntity> GetById(Guid id);

    Task<HashSet<Permission>> GetUserPermissions(Guid id);

    Task Update(Guid id, string fullName, DateTime birthDate);

    Task<List<UserEntity>> GetAll();
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
    
    public async Task Update(Guid id, string fullName, DateTime birthDate)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception($"User with id {id} does not exist");
        
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
    
    public async Task<UserEntity> GetById(Guid id)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception($"User with id {id} does not exist");

        return userEntity;
    }

    public async Task<HashSet<Permission>> GetUserPermissions(Guid id)
    {
        var roles = await _context.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .Where(u => u.Id == id)
            .Select(u => u.Roles)
            .ToArrayAsync();
        
        return roles
            .SelectMany(r => r)
            .SelectMany(r => r.Permissions)
            .Select(p => (Permission)p.Id)
            .ToHashSet();
    }
    
    public async Task<List<UserEntity>> GetAll()
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();
        
        return users;
    }
}