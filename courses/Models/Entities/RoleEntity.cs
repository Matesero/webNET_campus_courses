﻿namespace courses.Models.Entities;

public class RoleEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public List<PermissionEntity> Permissions { get; set; } = [];

    public List<UserEntity> Users { get; set; } = [];
}