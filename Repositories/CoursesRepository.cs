using courses.Models.Entities;
using courses.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface ICoursesRepository
{
    Task Add(CourseEntity courseEntity);

    Task Update(CourseEntity courseEntity);

    Task<CourseEntity> GetById(Guid id);
    
    Task<CourseEntity> GetByIdWithStudents(Guid id);

    Task Delete(CourseEntity courseEntity);

    Task<List<CourseEntity>> GetByStudentId(Guid id);

    Task<List<CourseEntity>> GetByTeacherId(Guid id);

    Task<CourseEntity> GetDetailedInfoById(Guid id);

    Task<List<CourseEntity>> GetByFiltersAndPagination(
        SortList? sort,
        string? search,
        bool? hasPlacesAndOpen,
        Semesters? semester,
        int page,
        int pageSize);
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
    
    public async Task Update(CourseEntity courseEntity)
    {
        _context.Courses.Update(courseEntity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<CourseEntity> GetById(Guid id)
    {
        return await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);;
    }
    
    public async Task<CourseEntity> GetByIdWithStudents(Guid id)
    {
        return await _context.Courses
            .AsNoTracking()
            .Include(c => c.Students
                .Where(s => s.Status == "Accepted"))
            .FirstOrDefaultAsync(c => c.Id == id);
    }
  
    public async Task Delete(CourseEntity courseEntity)
    {
        _context.Courses.Remove(courseEntity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<CourseEntity>> GetByStudentId(Guid id)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.UserId == id)
            .Join(
                _context.Courses,
                student => student.CourseId,
                course => course.Id,
                (student, course) => course)
            .Include(c => c.Students)
            .ToListAsync();;
    }
    
    public async Task<List<CourseEntity>> GetByTeacherId(Guid id)
    {
        return await _context.Teachers
            .AsNoTracking()
            .Where(t => t.UserId == id)
            .Join(
                _context.Courses,
                teacher => teacher.CourseId,
                course => course.Id,
                (teacher, course) => course)
            .ToListAsync();;
    }

    public async Task<CourseEntity> GetDetailedInfoById(Guid id)
    {
        return await _context.Courses
            .AsNoTracking()
            .Include(c => c.Teachers)
            .ThenInclude(t => t.User)
            .Include(c => c.Students)
            .ThenInclude(s=> s.User)
            .Include(c => c.Notifications)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task EditStatus(Guid id, string status)
    {
        var course = await _context.Courses.FindAsync(id);
        
        course.Status = status;
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<CourseEntity>> GetByFiltersAndPagination(
        SortList? sort,
        string? search,
        bool? hasPlacesAndOpen,
        Semesters? semester,
        int page,
        int pageSize)
    {
        var query = _context.Courses.AsNoTracking();

        query = query
            .Include(c => c.Students);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Name.Contains(search));
        }

        if (hasPlacesAndOpen.HasValue && hasPlacesAndOpen.Value)
        {
            query = query
                .Where(c => c.Status == "OpenForAssigning")
                .Where(c => c.Students.Count < c.MaximumStudentsCount);
        }

        if (semester.HasValue)
        {
            query = semester == Semesters.Autumn? 
                query.Where(c => c.Semester == "Autumn") : 
                query.OrderByDescending(c => c.Semester == "Spring");
        }

        if (sort.HasValue)
        {
            query = sort == SortList.CreatedAsc ? 
                query.OrderBy(c => c.CreatedDate) : 
                query.OrderByDescending(c => c.CreatedDate);
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}