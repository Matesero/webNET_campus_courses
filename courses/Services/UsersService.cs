using courses.Infrastructure;
using courses.Middleware;
using courses.Models.DTO;
using courses.Models.Entities;
using courses.Repositories;

namespace courses.Services;

public class UsersService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtProvider _jwtProvider;

    public UsersService(
        IUsersRepository usersRepository, 
        IPasswordHasher passwordHasher, 
        IJwtProvider jwtProvider)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }
    
    public async Task<TokenResponse> Register(
        string fullName, 
        DateTime birthDate, 
        string email, 
        string password)
    {
        var hashedPassword = _passwordHasher.Generate(password);
        
        var user = UserEntity.Create(
            Guid.NewGuid(), 
            fullName, 
            birthDate, 
            email, 
            hashedPassword);
        
        await _usersRepository.Add(user);
        
        var response = new TokenResponse 
        {
            token = _jwtProvider.GenerateToken(user)
        };
        
        return response;
    }
    
    public async Task<TokenResponse> Login(string email, string password)
    {
        var user = await _usersRepository.GetByEmail(email);

        if (user == null)
        {
            throw new NotFoundException(email, "User", "Email");
        }
        
        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (result == false)
        {
            throw new UnauthorizedException("The password is incorrect");
        }

        var response = new TokenResponse 
        {
            token = _jwtProvider.GenerateToken(user)
        };
        
        return response;
    }
    
    public async Task<UserProfileModel> GetProfile(Guid userId)
    {
        var profile = await _usersRepository.GetById(userId);

        return new UserProfileModel
        {
            fullName = profile.FullName,
            email = profile.Email,
            birthDate = profile.BirthDate,
        };
    } 
    
    public async Task<UserEntity> GetProfileByEmail(string email)
    {
        return await _usersRepository.GetByEmail(email);
    } 
    
    public async Task<UserProfileModel> EditProfile(Guid userId, string fullName, DateTime birthDate)
    {
        var profile = await _usersRepository.GetById(userId);
        
        profile.FullName = fullName;
        profile.BirthDate = birthDate;
        
        await _usersRepository.Update(userId, fullName, birthDate);

        return new UserProfileModel
        {
            fullName = fullName,
            email = profile.Email,
            birthDate = birthDate,
        };
    } 
    
    public async Task<List<UserShortModel>> GetAll(Guid userId)
    {
        var userRole = await _usersRepository.GetRoles(userId);

        if (userRole.isAdmin || userRole.isTeacher)
        {
            var users = await _usersRepository.GetAll();

            return users.Select(u => new UserShortModel
            {
                id = u.Id,
                fullName = u.FullName,
            }).ToList();
            
        }
        
        throw new ForbiddenException(); 
    }

    public async Task<UserRolesModel> GetRoles(Guid userId)
    {
        return await _usersRepository.GetRoles(userId);
    }
}