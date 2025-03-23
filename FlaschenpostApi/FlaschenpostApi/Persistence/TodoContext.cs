using FlaschenpostApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlaschenpostApi.Persistence;

public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
            .HasKey(entity => entity.Id);
    }

    public DbSet<TodoItem> TodoItems { get; set; }
}