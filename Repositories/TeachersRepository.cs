using courses.Models.DTO;
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
}