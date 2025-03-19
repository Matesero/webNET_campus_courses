using courses.Middleware;
using courses.Models.DTO;
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
    
    Task<UserRolesModel> GetRoles(Guid userId);
    
    Task CheckExistence(Guid id);

    Task<RoleHierarchy> GetRoleHierarchy(Guid courseId, Guid userId);
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
            .FirstOrDefaultAsync(u => u.Id == id) ?? 
                   throw new NotFoundException(id.ToString(), "User", "ID");
        
        user.FullName = fullName;
        user.BirthDate = birthDate;
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<UserEntity> GetByEmail(string email)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        return userEntity;
    }
    
    public async Task<UserEntity> GetById(Guid id)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id) ?? 
                         throw new NotFoundException(id.ToString(), "User", "ID");

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
    
    public async Task<RoleHierarchy> GetRoleHierarchy(Guid courseId, Guid userId)
    {
        var isAdmin = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Include(u => u.Roles)
            .SelectMany(u => u.Roles.Select(r => r.Id))
            .ToListAsync();
    
        if (isAdmin.Contains((int)Role.Admin))
        {
            return RoleHierarchy.admin;
        }

        var teacher = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == courseId)
            .SelectMany(c => c.Teachers.Where(t => t.UserId == userId))
            .FirstOrDefaultAsync();

        if (teacher != null)
        {
            return teacher.IsMain ? RoleHierarchy.mainTeacher : RoleHierarchy.teacher;
        }

        var student = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == courseId)
            .SelectMany(c => c.Students.Where(s => s.UserId == userId))
            .FirstOrDefaultAsync();

        if (student != null)
        {
            return student.Status == "Accepted" ? RoleHierarchy.acceptedStudent : RoleHierarchy.student;
        }

        return RoleHierarchy.user;
    }

    public async Task<UserRolesModel> GetRoles(Guid userId)
    {
        var isAdmin = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Include(u => u.Roles)
            .SelectMany(u => u.Roles.Select(r => r.Id))
            .ToListAsync();

        var teacherQuery = await _context.Teachers
            .Where(t => t.UserId == userId)
            .Select(t => t.UserId)
            .AnyAsync();;
        
        var studentQuery = await _context.Students
            .Where(s => s.UserId == userId)
            .Select(s => s.UserId)
            .AnyAsync();

        return new UserRolesModel
        {
            isAdmin = isAdmin.Contains((int)Role.Admin),
            isTeacher = teacherQuery,
            isStudent = studentQuery,
        };
    }
    
    public async Task CheckExistence(Guid id)
    {
        var group = await _context.Users
            .AsNoTracking()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();

        if (group is null)
        { 
            throw new NotFoundException(id.ToString(), "User", "ID");
        }
    }
}