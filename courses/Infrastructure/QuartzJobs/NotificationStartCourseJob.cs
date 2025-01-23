using courses.Infrastructure.SenderMessages;
using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace courses.Infrastructure.QuartzJobs;

public class NotificationStartCourseJob: IJob
{
    private readonly CoursesDbContext _context;
    private readonly ISenderMessages _senderMessages;
    private readonly int _batchSize = 20;
    
    public NotificationStartCourseJob(
        CoursesDbContext context, 
        ISenderMessages senderMessages)
    {
        _context = context;
        _senderMessages = senderMessages;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var today = DateTime.UtcNow;

        var coursesQuery = _context.Courses
            .Where(course => course.StartYear == DateTime.Today.Year && course.Status != "Finished" &&
                             course.Status != "Started");

        if (today.Month == 2)
        {
            coursesQuery = coursesQuery.Where(course => course.Semester == "Spring");
        }
        else
        {
            coursesQuery = coursesQuery.Where(course => course.Semester == "Autumn");
        }

        var courses = await coursesQuery
            .Include(course => course.Students
                .Where(student => student.Status == "Accepted"))
            .ThenInclude(student => student.User)
            .ToListAsync();

        int batchCount = 0;
        
        foreach (var course in courses)
        {
            foreach (var student in course.Students)
            {
                var email = student.User.Email;
                var subject = $"Курс {course.Name} начнется завтра!";
                var body = $"Здравствуйте, {student.User.FullName}!\n\n" +
                           $"Напоминаем, что завтра начнется курс \"{course.Name}\". " +
                           $"Пожалуйста, убедитесь, что вы готовы.\n\n" +
                           "С уважением, команда организаторов.";
            
                _senderMessages.SendMessage(email, subject, body);
            
                batchCount++;
                
                await Task.Delay(500);
            }

            if (batchCount >= _batchSize)
            {
                batchCount = 0;
                await Task.Delay(5000);
            }
        }
    }
}

