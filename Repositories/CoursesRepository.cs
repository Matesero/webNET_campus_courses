using courses.Models.DTO;
using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface ICoursesRepository
{
    Task Add(CourseEntity courseEntity);
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
  
}