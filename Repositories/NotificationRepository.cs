using courses.Models.DTO;
using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace courses.Repositories;

public interface INotificationsRepository
{
    Task Add(NotificationEntity notificationEntity);
}

public class NotificationsRepository : INotificationsRepository
{
    private readonly CoursesDbContext _context;

    public NotificationsRepository(CoursesDbContext context)
    {
        _context = context;
    }
    
    public async Task Add(NotificationEntity notificationEntity)
    {
        await _context.Notifications.AddAsync(notificationEntity);
        await _context.SaveChangesAsync();
    }
}