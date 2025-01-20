using courses.Models.DTO;
using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface IStudentsRepository
{
    Task Add(StudentEntity studentEntity);
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
        try
        {
            await _context.Students.AddAsync(studentEntity);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null && e.InnerException.Message.Contains("duplicate key value violates unique constraint"))
            {
                throw new InvalidOperationException("This user is already assigned to the course");
            }
        }
        
    }
}