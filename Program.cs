using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CoursesDbContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(CoursesDbContext)));
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUi();