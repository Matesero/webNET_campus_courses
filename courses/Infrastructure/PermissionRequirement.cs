using courses.Models.enums;
using Microsoft.AspNetCore.Authorization;

namespace courses.Infrastructure;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(Permission[] permissions)
    {
        Permissions = permissions;
    }
    
    public Permission[] Permissions { get; set; } = [];
}