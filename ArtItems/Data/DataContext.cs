using System;
using ArtItems.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtItems.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public string DbPath { get; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
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