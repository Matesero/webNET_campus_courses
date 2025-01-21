using courses.Infrastructure;
using courses.Models.DTO;
using courses.Models.Entities;
using courses.Repositories;

namespace courses.Services;

public class UsersService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtProvider _jwtProvider;

    public UsersService(IUsersRepository usersRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }
    
    public async Task<TokenResponse> Register(string fullName, DateTime birthDate, string email, string password, string confirmPassword)
    {
        var hashedPassword = _passwordHasher.Generate(password);
        
        var user = UserEntity.Create(Guid.NewGuid(), fullName, birthDate, email, hashedPassword);
        
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

        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (result == false)
        {
            throw new Exception(); // обработать
        }

        var response = new TokenResponse 
        {
            token = _jwtProvider.GenerateToken(user)
        };
        
        return response;
    }
    public async Task<UserProfileModel> GetProfile(string userId)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            throw new Exception(); // обработать
        }

        var profile = await _usersRepository.GetById(id);

        if (profile is null)
        {
            throw new Exception(); // обработать
        }

        return new UserProfileModel
        {
            fullName = profile.FullName,
            email = profile.Email,
            birthDate = profile.BirthDate,
        };
    } 
    
    public async Task<UserProfileModel> EditProfile(string userId, string fullName, DateTime birthDate)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            throw new Exception(); // обработать
        }

        var profile = await _usersRepository.GetById(id);

        if (profile is null)
        {
            throw new Exception(); // обработать
        }
        
        await _usersRepository.Update(id, fullName, birthDate);

        return new UserProfileModel
        {
            fullName = fullName,
            email = profile.Email,
            birthDate = birthDate,
        };
    } 
    
    public async Task<List<UserShortModel>> GetAll()
    {
        var users = await _usersRepository.GetAll();

        return users.Select(u => new UserShortModel
        {
            id = u.Id,
            fullName = u.FullName,
        }).ToList();
    }

    public async Task<UserRolesModel> GetRoles(string userId)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            throw new Exception(); // обработать
        }

        return await _usersRepository.GetRoles(id);
    }
}