using System.Text.Json.Serialization;
using courses.Extensions;
using courses.Infrastructure;
using courses.Infrastructure.QuartzJobs;
using courses.Infrastructure.SenderMessages;
using courses.Models.DTO;
using courses.Models.Entities;
using courses.Repositories;
using courses.Services;
using courses.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
services.Configure<AuthorizationOptions>(configuration.GetSection("AuthorizationOptions"));
services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
services.Configure<UserLoginModel>(configuration.GetSection("SmtpSettings"));

services.AddApiAuthentication(configuration);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<CoursesDbContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString("CoursesDbContext"));
    });

services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IGroupsRepository, GroupsRepository>();
services.AddScoped<ICoursesRepository, CoursesRepository>();
services.AddScoped<ITeachersRepository, TeachersRepository>();
services.AddScoped<IStudentsRepository, StudentsRepository>();
services.AddScoped<INotificationsRepository, NotificationsRepository>();
services.AddScoped<IBlackTokensRepository, BlackTokensRepository>();
services.AddScoped<ISenderMessages, SenderMessages>();

services.AddValidatorsFromAssemblyContaining<RegistrationValidator>();
services.AddValidatorsFromAssemblyContaining<LoginValidator>();
services.AddValidatorsFromAssemblyContaining<EditProfileValidator>();
services.AddValidatorsFromAssemblyContaining<CreateGroupValidator>();
services.AddValidatorsFromAssemblyContaining<EditGroupValidator>();
services.AddValidatorsFromAssemblyContaining<CreateCourseValidator>();
services.AddValidatorsFromAssemblyContaining<FilterCoursesValidator>();
services.AddValidatorsFromAssemblyContaining<EditCourseStatusValidator>();
services.AddValidatorsFromAssemblyContaining<EditCampusCourseRequirementsAndAnnotationsValidator>();
services.AddValidatorsFromAssemblyContaining<CreateCourseNotificationValidator>();
services.AddValidatorsFromAssemblyContaining<EditStudentMarkValidator>();
services.AddValidatorsFromAssemblyContaining<EditStudentStatusValidator>();

services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddQuartz(quartz =>
{
    quartz.UseMicrosoftDependencyInjectionJobFactory(); 
    
    var jobKey = new JobKey("NotificationStartCourseJob");
    quartz.AddJob<NotificationStartCourseJob>(opts => opts.WithIdentity(jobKey));  
    quartz.AddTrigger(opts => opts
        .ForJob(jobKey)  
        .WithIdentity("NotificationStartCourseTrigger")
        .WithCronSchedule("0 0 12 L FEB,AUG ? *")); 
    
    var jobKey2 = new JobKey("ClearBlackTokensJob");
    quartz.AddJob<ClearBlackTokensJob>(opts => opts.WithIdentity(jobKey2));
    quartz.AddTrigger(opts => opts
        .ForJob(jobKey2)
        .WithIdentity("ClearBlackTokensTrigger")
        .WithCronSchedule("0 0 23 ? * * *"));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

services.AddScoped<UsersService>();
services.AddScoped<GroupsService>();
services.AddScoped<CoursesService>();
services.AddScoped<BlackTokensService>();

services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();
    dbContext.MigrateDatabase();

    if (!dbContext.Users.Any())
    {
        var adminUser = UserEntity.Create(
            Guid.NewGuid(),
            "Admin",
            DateTime.Now.AddYears(-10).ToUniversalTime(),
            "admin@example.com",
            BCrypt.Net.BCrypt.EnhancedHashPassword("admin@example.com")
        );

        var adminRole = dbContext.Roles.FirstOrDefault(role => role.Name == "Admin");

        adminUser.Roles.Add(adminRole);
        dbContext.Users.Add(adminUser);
        
        dbContext.SaveChanges();
    }
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddlewareHandlerException();
app.UseAuthentication();
app.UseAuthorization();

app.AddMappedEndpoints();

app.Run();