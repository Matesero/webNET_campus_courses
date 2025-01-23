using courses.Extensions;
using courses.Services;
using Microsoft.AspNetCore.Authorization;

namespace courses.Infrastructure;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userId = context.User.GetUserId();
        
        using var scope = _serviceScopeFactory.CreateScope();
        
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        var permissions = await permissionService.GetPermissionsAsync(userId);
        
        if (permissions.Intersect(requirement.Permissions).Any())
        {
            context.Succeed(requirement);
        }
    }
}