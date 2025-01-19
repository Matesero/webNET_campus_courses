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
    
    public async Task Register(string fullName, DateTime birthDate, string email, string password, string confirmPassword)
    {
        var hashedPassword = _passwordHasher.Generate(password);
        
        var user = UserEntity.Create(Guid.NewGuid(), fullName, birthDate, email, hashedPassword);
        
        await _usersRepository.Add(user);
    }
    
    public async Task<TokenResponse> Login(string email, string password)
    {
        var user = await _usersRepository.GetByEmail(email);

        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (result == false)
        {
            throw new Exception();
        }

        var response = new TokenResponse 
        {
            token = _jwtProvider.GenerateToken(user)
        };
        
        return response;
    }
}