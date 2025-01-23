using courses.Models.DTO;
using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface IStudentsRepository
{
    Task Add(StudentEntity studentEntity);
    
    Task Update(StudentEntity studentEntity);
}

public class StudentsRepository : IStudentsRepository
{
    private readonly CoursesDbContext _context;

    public StudentsRepository(CoursesDbContext context)
    {
        _context = context;
    }
    
    public async Task Add(StudentEntity studentEntity)
    {
        await _context.Students.AddAsync(studentEntity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(StudentEntity studentEntity)
    {
        _context.Students.Update(studentEntity);
        await _context.SaveChangesAsync();
    }
}