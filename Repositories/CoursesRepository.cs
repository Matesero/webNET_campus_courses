using System.Xml;
using courses.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface ICoursesRepository
{
    Task Add(CourseEntity courseEntity);

    Task<CourseEntity> GetById(Guid id);

    Task Delete(Guid id);
}

public class CoursesRepository : ICoursesRepository
{
    private readonly CoursesDbContext _context;

    public CoursesRepository(CoursesDbContext context)
    {
        _context = context;
    }
    
    public async Task Add(CourseEntity courseEntity)
    {
        await _context.Courses.AddAsync(courseEntity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<CourseEntity> GetById(Guid id)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return course;
    }
  
    public async Task Delete(Guid id)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id); 
        
        _context.Courses.Remove(course);
        
        await _context.SaveChangesAsync();
    }

    [Authorize]
    public async Task SignUpCourse(Guid id)
    {
        
    }
}