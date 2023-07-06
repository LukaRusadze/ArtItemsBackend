using System;
using System.ComponentModel.DataAnnotations;

namespace ArtItems.Models;

public class User
{
    [Key]
    public int ID { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string Username { get; set; } = "";
}

