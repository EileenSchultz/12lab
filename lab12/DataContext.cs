namespace 12lab;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
        optionsBuilder.LogTo(Console.WriteLine);
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }
    
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<User> Users => Set<User>();
}