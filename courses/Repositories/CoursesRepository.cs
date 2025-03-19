using courses.Middleware;
using courses.Models.Entities;
using courses.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface ICoursesRepository
{
    Task Add(CourseEntity courseEntity);

    Task Update(CourseEntity courseEntity);

    Task<CourseEntity> GetById(Guid id);
    
    Task CheckExistence(Guid id);
    
    Task<CourseEntity> GetByIdWithStudentsAndTeachers(Guid id);

    Task Delete(Guid id);

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
    
    public async Task<CourseEntity> GetByIdWithStudentsAndTeachers(Guid id)
    {
        return await _context.Courses
            .AsNoTracking()
            .Include(c => c.Students)
            .Include(c => c.Teachers)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
  
    public async Task Delete(Guid id)
    {
        var course = await _context.Courses
                        .FirstOrDefaultAsync(g => g.Id == id) ?? 
                    throw new NotFoundException(id.ToString(), "Course", "ID");
            
        _context.Courses.Remove(course);
        
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
            .FirstOrDefaultAsync(c => c.Id == id) ?? 
               throw new NotFoundException(id.ToString(), "Course", "ID");
    }
    
    public async Task CheckExistence(Guid id)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();

        if (course is null)
        { 
            throw new NotFoundException(id.ToString(), "Course", "ID");
        }
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
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()));
        }

        query = query
            .Include(c => c.Students);

        if (hasPlacesAndOpen.HasValue && hasPlacesAndOpen.Value)
        {
            query = query
                .Where(c => c.Status == "OpenForAssigning" &&
                            c.Students.Count(s =>
                                s.Status == Enum.GetName(StudentStatuses.Accepted)) < c.MaximumStudentsCount);
        }
        else if (hasPlacesAndOpen.HasValue && !hasPlacesAndOpen.Value)
        {
            query = query
                .Where(c => c.Status != "OpenForAssigning" ||
                            c.Students.Count(s =>
                                s.Status == Enum.GetName(StudentStatuses.Accepted)) >= c.MaximumStudentsCount);
        }

        if (semester.HasValue)
        {
            query = semester == Semesters.Autumn? 
                query.Where(c => c.Semester == "Autumn") : 
                query.Where(c => c.Semester == "Spring");
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