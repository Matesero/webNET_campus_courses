using courses.Models.enums;
using courses.Repositories;

namespace courses.Services;

public interface IPermissionService
{
    Task<HashSet<Permission>> GetPermissionsAsync(Guid userId);
}

public class PermissionService : IPermissionService
{
    private readonly IUsersRepository _usersRepository;

    public PermissionService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public Task<HashSet<Permission>> GetPermissionsAsync(Guid userId)
    {
        return _usersRepository.GetUserPermissions(userId);
    }
}