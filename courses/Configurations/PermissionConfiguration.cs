using courses.Models.Entities;
using courses.Models.enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace courses.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.
            HasMany(p => p.Roles)
            .WithMany(r => r.Permissions)
            .UsingEntity<RolePermissionEntity>(
                l => l.HasOne<RoleEntity>().WithMany().HasForeignKey(r => r.RoleId),
                r => r.HasOne<PermissionEntity>().WithMany().HasForeignKey(p => p.PermissionId));

        var permissions = Enum
            .GetValues<Permission>()
            .Select(p => new PermissionEntity
            {
                Id = (int)p,
                Name = p.ToString(),
            });
        
        builder.HasData(permissions);
    }
}