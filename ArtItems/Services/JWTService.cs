using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ArtItems.Models;

namespace ArtItems.Services;

public interface IJWTService
{
    string GenerateSecurityToken(User user);
}

public class JWTService : IJWTService
{
    private string _secret = "";
    private int _expirationInMinutes;

    public JWTService(IConfiguration config)
    {
        var secretKey = config.GetSection("jwt").GetValue<string>("secret");

        if (secretKey == null)
        {
            throw new Exception("jwt secret not found");
        }

        _secret = secretKey;
        _expirationInMinutes = config.GetSection("jwt").GetValue<int>("expirationInMinutes");
    }

    public string GenerateSecurityToken(User user)
    {

        var claims = new[]
        {
            new Claim("id", user.ID.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("username", user.Username),
        };


        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow.AddMinutes(_expirationInMinutes),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenOptions);
        return tokenHandler.WriteToken(token);
    }
}

