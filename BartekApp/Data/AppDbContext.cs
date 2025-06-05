using BartekApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BartekApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string obPath = Path.Combine(FileSystem.AppDataDirectory, "budget.db");
        optionsBuilder.UseSqlite($"Data Source={obPath}");
    }
}