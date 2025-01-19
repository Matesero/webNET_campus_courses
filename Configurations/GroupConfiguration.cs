using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace courses.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<GroupEntity>
{
    public void Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.HasKey(g => g.Id);

        builder.
            HasMany(g => g.Courses)
            .WithOne(c => c.Group)
            .HasForeignKey(c => c.GroupId);
    }
}