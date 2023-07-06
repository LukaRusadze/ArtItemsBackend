using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ArtItems.Models;
using ArtItems.Data;
using ArtItems.Services;
using System.IdentityModel.Tokens.Jwt;

namespace ArtItems.Controllers;

public class UserRegister
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public interface IRegisterResponse
{
    public string hashedPassword { get; set; }
}


[ApiController]
public class UserController : ControllerBase
{
    private readonly PasswordHasher<string> hasher = new();
    private readonly IJWTService _jwtService;

    public UserController(IJWTService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost]
    [Route("[controller]/register")]
    public async Task<IActionResult> Register(UserRegister newUser)
    {
        using var db = new DataContext();
        var hashedPassword = hasher.HashPassword(newUser.Email, newUser.Password);
        var generatedUser = new User() { Email = newUser.Email, Password = hashedPassword };
        var added = await db.AddAsync(generatedUser);
        await db.SaveChangesAsync();
        return Ok(added.Entity);
    }

    [HttpPost]
    [Route("[controller]/login")]
    public IActionResult Login(UserRegister user)
    {
        using var db = new DataContext();
        try
        {
            var registeredUser = db.Users.First((item) => item.Email == user.Email);
            var result = hasher.VerifyHashedPassword(user.Email, registeredUser.Password, user.Password);

            if (result != PasswordVerificationResult.Success) return NotFound();

            var jwtToken = _jwtService.GenerateSecurityToken(registeredUser);

            return Ok(new { user = registeredUser, access_token = jwtToken });

        }
        catch (Exception ex)
        {
            return NotFound(ex);
        }
    }
}

