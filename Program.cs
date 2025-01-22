using System.Text.Json.Serialization;
using courses.Extensions;
using courses.Infrastructure;
using courses.Models.Entities;
using courses.Repositories;
using courses.Services;
using courses.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
services.Configure<AuthorizationOptions>(configuration.GetSection("AuthorizationOptions"));

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

services.AddValidatorsFromAssemblyContaining<RegistrationValidator>();
services.AddValidatorsFromAssemblyContaining<LoginValidator>();
services.AddValidatorsFromAssemblyContaining<EditProfileValidator>();
services.AddValidatorsFromAssemblyContaining<CreateGroupValidator>();
services.AddValidatorsFromAssemblyContaining<CreateCourseValidator>();

services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

services.AddScoped<UsersService>();
services.AddScoped<GroupsService>();
services.AddScoped<CoursesService>();
services.AddScoped<BlackTokensService>();

services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddlewareHandlerException();
app.UseAuthentication();
app.UseAuthorization();

app.AddMappedEndpoints();

app.Run();