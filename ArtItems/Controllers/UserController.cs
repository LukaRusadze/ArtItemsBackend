using System;
using System.IdentityModel.Tokens.Jwt;
using ArtItems.Data;
using ArtItems.Models;
using ArtItems.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtItems.Controllers;

public class UserRegister
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public interface IRegisterResponse
{
    public string HashedPassword { get; set; }
}


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly PasswordHasher<string> _hasher = new();
    private readonly IJWTService _jwtService;
    private readonly DataContext _db;

    public UserController(IJWTService jwtService, DataContext db)
    {
        _jwtService = jwtService;
        _db = db;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(UserRegister newUser)
    {
        var hashedPassword = _hasher.HashPassword(newUser.Email, newUser.Password);
        var generatedUser = new User() { Email = newUser.Email, Password = hashedPassword };
        var added = await _db.AddAsync(generatedUser);
        await _db.SaveChangesAsync();
        return Ok(added.Entity);
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login(UserRegister user)
    {
        try
        {
            var registeredUser = _db.Users.First((item) => item.Email == user.Email);
            var result = _hasher.VerifyHashedPassword(user.Email, registeredUser.Password, user.Password);

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

