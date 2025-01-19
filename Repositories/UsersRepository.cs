using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface IUsersRepository
{
    Task Add(UserEntity user);
    
    Task<UserEntity> GetByEmail(string email);
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
    
    public async Task<UserEntity> GetByEmail(string email)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email) ?? throw new Exception($"User with email {email} does not exist");

        return userEntity;
    }
}