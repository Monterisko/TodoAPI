using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Context;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<Todo> Todos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>()
            .Property(t => t.ExpiryDate)
            .HasColumnType("timestamp without time zone");
    }
}