using Microsoft.EntityFrameworkCore;
using MinimalApiApplication.Models;

namespace MinimalApiApplication.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Item> Items { get; set; }
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(e =>
        {
            e.Property(i=>i.Name).HasMaxLength(100);
            e.Property(i=>i.Price).HasColumnType("decimal(10,2)");
        });
    }
}
