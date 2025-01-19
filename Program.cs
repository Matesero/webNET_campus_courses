using courses.Endpoints;
using courses.Extensions;
using courses.Infrastructure;
using courses.Models.Entities;
using courses.Models.enums;
using courses.Repositories;
using courses.Services;
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

services.AddScoped<UsersService>();
services.AddScoped<GroupsService>();

services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.AddMappedEndpoints();

app.MapGet("get", () =>
{
    return Results.Ok("ok");
}).RequirePermissions(Permission.Read);

app.Run();