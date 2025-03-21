﻿namespace courses.Infrastructure;

public class AuthorizationOptions
{
    public RolePermissions[] RolePermissions { get; set; } = [];
}

public class RolePermissions
{
    public string Role { get; set; }

    public string[] Permissions { get; set; } = [];
}