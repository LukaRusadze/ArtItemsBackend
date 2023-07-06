using System;
using Microsoft.EntityFrameworkCore;
using ArtItems.Models;

namespace ArtItems.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public string DbPath { get; }

    public DataContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "users.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }
}