using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace courses.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<CourseEntity>
{
    public void Configure(EntityTypeBuilder<CourseEntity> builder)
    {
        builder.HasKey(c => c.Id);

        builder.
            HasOne(c => c.Group)
            .WithMany(c => c.Courses)
            .HasForeignKey(t => t.GroupId);

        builder.
            HasMany(c => c.Teachers)
            .WithOne(t => t.Course)
            .HasForeignKey(t => t.CourseId);
        
        builder.
            HasMany(c => c.Students)
            .WithOne(t => t.Course)
            .HasForeignKey(t => t.CourseId);
        
        builder.
            HasMany(c => c.Notifications)
            .WithOne(t => t.Course)
            .HasForeignKey(t => t.CourseId);
    }
}