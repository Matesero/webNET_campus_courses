using courses.Configurations;
using Microsoft.EntityFrameworkCore;

namespace courses.Models.Entities;

public class CoursesDbContext(DbContextOptions<CoursesDbContext> options) : DbContext
{
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<TeacherEntity> Teachers { get; set; }
    public DbSet<StudentEntity> Students { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());

        base.OnModelCreating(modelBuilder);
    }

}