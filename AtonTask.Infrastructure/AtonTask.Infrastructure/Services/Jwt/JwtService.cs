using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AtonTask.Application.Contracts;
using AtonTask.Application.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace AtonTask.Infrastructure.Services.Jwt;

public class JwtService(JwtOptions jwtOptions) : IJwtService
{
    public string GenerateToken(UserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Guid.ToString()),
            new(ClaimTypes.Name, user.Login)
        };

        claims.Add(new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "User"));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}