using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Index on IsComplete for filtering
        modelBuilder
            .Entity<TodoItem>()
            .HasIndex(t => t.IsComplete)
            .HasDatabaseName("IX_TodoItems_IsComplete");

        // Index on Name for searching
        modelBuilder.Entity<TodoItem>().HasIndex(t => t.Name).HasDatabaseName("IX_TodoItems_Name");

        // Composite index for filtering + pagination
        modelBuilder
            .Entity<TodoItem>()
            .HasIndex(t => new { t.IsComplete, t.Id })
            .HasDatabaseName("IX_TodoItems_IsComplete_Id");
    }
}
