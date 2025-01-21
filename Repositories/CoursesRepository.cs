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

    Task<List<CourseEntity>> GetByStudentId(Guid id);

    Task<List<CourseEntity>> GetByTeacherId(Guid id);
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
    
    public async Task<List<CourseEntity>> GetByStudentId(Guid id)
    {
        var courses = await _context.Students
            .AsNoTracking()
            .Where(s => s.UserId == id)
            .Join(
                _context.Courses,
                student => student.CourseId,
                course => course.Id,
                (student, course) => course)
            .ToListAsync();
        
        return courses;
    }
    
    public async Task<List<CourseEntity>> GetByTeacherId(Guid id)
    {
        var courses = await _context.Teachers
            .AsNoTracking()
            .Where(t => t.UserId == id)
            .Join(
                _context.Courses,
                teacher => teacher.CourseId,
                course => course.Id,
                (teacher, course) => course)
            .ToListAsync();
        
        return courses;
    }
}