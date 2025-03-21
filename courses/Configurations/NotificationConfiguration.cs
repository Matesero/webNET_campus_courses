﻿using courses.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace courses.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.HasKey(n => n.Id);
        
        builder.
            HasOne(n => n.Course)
            .WithMany(c => c.Notifications)
            .OnDelete(DeleteBehavior.Cascade);
    }
}