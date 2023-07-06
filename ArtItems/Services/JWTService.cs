using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArtItems.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ArtItems.Services;

public interface IJWTService
{
    string GenerateSecurityToken(User user);
}

public class JWTService : IJWTService
{
    private readonly string _secret;
    private readonly int _expirationInMinutes;

    public JWTService(IOptions<Config> config)
    {
        var secretKey = config.Value.Secret;

        if (secretKey == null)
        {
            throw new Exception("jwt secret not found");
        }

        _secret = secretKey;
        _expirationInMinutes = config.Value.ExpirationInMinutes;
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

