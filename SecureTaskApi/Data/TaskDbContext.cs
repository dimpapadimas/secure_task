using Microsoft.EntityFrameworkCore;
using SecureTaskApi.Models;

namespace SecureTaskApi.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Priority).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();
            
            // Seed some initial data
            entity.HasData(
                new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Setup development environment",
                    Description = "Install .NET SDK, Visual Studio, and configure Git",
                    Priority = TaskPriority.High,
                    Status = TaskStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CompletedAt = DateTime.UtcNow.AddDays(-4)
                },
                new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Implement authentication",
                    Description = "Add JWT-based authentication to the API",
                    Priority = TaskPriority.Critical,
                    Status = TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Write unit tests",
                    Description = "Create unit tests for all controllers and services",
                    Priority = TaskPriority.High,
                    Status = TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );
        });
    }
}
