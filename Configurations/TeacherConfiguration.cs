using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace courses.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<TeacherEntity>
{
    public void Configure(EntityTypeBuilder<TeacherEntity> builder)
    {
        builder.HasKey(s => new {s.UserId, s.CourseId});

        builder.
            HasOne(s => s.Course)
            .WithMany(c => c.Teachers)
            .HasForeignKey(s => s.CourseId);
        
        builder
            .HasOne(s => s.Group)
            .WithMany()
            .HasForeignKey(s => s.GroupId);
    }
}