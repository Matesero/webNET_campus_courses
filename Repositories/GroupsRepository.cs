using System.Collections.Specialized;
using System.Text.RegularExpressions;
using courses.Models.DTO;
using courses.Models.Entities;
using courses.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface IGroupsRepository
{
    Task<GroupEntity> GetById(Guid id);
    
    Task<List<CampusGroupModel>> GetAll();
    
    Task Add(GroupEntity groupEntity);

    Task Update(Guid id, string name);

    Task Delete(Guid id);

    Task<List<CourseEntity>> GetCourses(Guid id);

    Task<List<TeacherEntity>> GetWithDetailedCourses(string semester, List<Guid> groupsIds);
}

public class GroupsRepository : IGroupsRepository
{
    private readonly CoursesDbContext _context;

    public GroupsRepository(CoursesDbContext context)
    {
        _context = context;
    }
    
    public async Task<GroupEntity> GetById(Guid id)
    {
        var group = await _context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

        return group;
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
    
    public async Task Add(GroupEntity groupEntity)
    {
        await _context.Groups.AddAsync(groupEntity);
        await _context.SaveChangesAsync();
    }
    
    public async Task Update(Guid id, string name)
    {
        var group = await _context.Groups
            .FirstOrDefaultAsync(g => g.Id == id); 
            
        group.Name = name;
        
        await _context.SaveChangesAsync();
    }
    
    public async Task Delete(Guid id)
    {
        var group = await _context.Groups
            .FirstOrDefaultAsync(g => g.Id == id); 
            
        _context.Groups.Remove(group);
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<CourseEntity>> GetCourses(Guid id)
    {
        var courses = await _context.Courses
            .AsNoTracking()
            .Where(c => c.GroupId == id)
            .Include(c => c.Students
                .Where(s => s.Status == "Accepted"))
            .ToListAsync();
        
        return courses;
    }

    public async Task<List<TeacherEntity>> GetWithDetailedCourses(string semester, List<Guid> groupsIds)
    {
        var query = _context.Teachers.AsNoTracking();

        query = query.Where(t => t.IsMain == true);
        
        query = query
            .Include(t => t.User)
            .OrderBy(t => t.User.FullName);

        query = query
            .Include(t => t.Group)
            .ThenInclude(g => g.Courses
                .Where(c => string.IsNullOrWhiteSpace(semester) || c.Semester == semester))
            .ThenInclude(g => g.Students)
            .Where(t => groupsIds.Contains(t.Group.Id) || groupsIds.Count() == 0);
        
        return await query.ToListAsync();
    }
}