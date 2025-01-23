using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface ITeachersRepository
{
    Task Add(TeacherEntity teacherEntity);
}

public class TeachersRepository : ITeachersRepository
{
    private readonly CoursesDbContext _context;

    public TeachersRepository(CoursesDbContext context)
    {
        _context = context;
    }
    
    public async Task Add(TeacherEntity teacherEntity)
    {
        await _context.Teachers.AddAsync(teacherEntity);
        await _context.SaveChangesAsync();
    }
    
    public async Task GetByCourseAndUserIds(Guid courseId, Guid userId)
    {
        await _context.Teachers.AsNoTracking()
            .Where(t => t.CourseId == courseId && t.UserId == userId)
            .FirstOrDefaultAsync();
        await _context.SaveChangesAsync();
    }
}