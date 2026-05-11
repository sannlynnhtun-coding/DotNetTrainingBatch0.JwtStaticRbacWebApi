using DotNetTrainingBatch0.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace DotNetTrainingBatch0.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> TblUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convert List<string> to a comma-separated string for storage in In-Memory DB
        modelBuilder.Entity<AppUser>()
            .Property(x => x.Permissions)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

    }
}
