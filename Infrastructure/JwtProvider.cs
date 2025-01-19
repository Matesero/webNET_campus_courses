using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using blog.Features.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace courses.Infrastructure;

public interface IJwtProvider
{
    string GenerateToken(UserEntity entity);
}

public class JwtProvider(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _options = options.Value;
    
    public string GenerateToken(UserEntity entity)
    {
        Claim[] claims = [new("userId", entity.Id.ToString())];
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_options.ExpireHours));
        
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        
        return tokenValue;
    }
}