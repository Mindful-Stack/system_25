using Microsoft.EntityFrameworkCore;
using TodoDemo.Models;

namespace TodoDemo.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options){ }

    public DbSet<TodoItem> Todos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.Title).IsRequired().HasMaxLength(200);
            b.Property(t => t.Description).HasMaxLength(500);
            b.Property(t => t.IsDone).HasDefaultValue(false);
            b.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
        
    }
}