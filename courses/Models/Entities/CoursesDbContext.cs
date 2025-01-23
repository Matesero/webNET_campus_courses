using courses.Configurations;
using courses.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace courses.Models.Entities;

public class CoursesDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<AuthorizationOptions> _authOptions;

    public CoursesDbContext(
        DbContextOptions<CoursesDbContext> options, 
        IOptions<AuthorizationOptions> authOptions, 
        IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
        _authOptions = authOptions;
    }
    
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<TeacherEntity> Teachers { get; set; }
    public DbSet<StudentEntity> Students { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<BlackTokenEntity> BlackTokens { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("CoursesDbContext");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(_authOptions.Value));
        
        base.OnModelCreating(modelBuilder);
    }
    
    public void MigrateDatabase()
    {
        Database.Migrate();
    }
}