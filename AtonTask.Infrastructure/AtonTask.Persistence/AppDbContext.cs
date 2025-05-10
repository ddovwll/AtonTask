using System.Reflection;
using AtonTask.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AtonTask.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<RefreshModel> Refreshes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}