using BlazorShowcase.Accounts;
using BlazorShowcase.Employees;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlazorShowcase.DataAccess;

public class DbCon : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(BlazorShowcase)}Db.db");
        optionsBuilder.UseSqlite($"Data Source={file}");
    }
}
