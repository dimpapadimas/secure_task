using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SecureTaskApi.Data;
using SecureTaskApi.Models;
using SecureTaskApi.Services;
using Xunit;

namespace SecureTaskApi.Tests;

public class TaskServiceTests : IDisposable
{
    private readonly TaskDbContext _context;
    private readonly TaskService _service;
    private readonly Mock<ILogger<TaskService>> _loggerMock;

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TaskDbContext(options);
        _loggerMock = new Mock<ILogger<TaskService>>();
        _service = new TaskService(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnAllTasks()
    {
        // Arrange
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Task 1",
            Priority = TaskPriority.High,
            Status = Models.TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        var task2 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Task 2",
            Priority = TaskPriority.Low,
            Status = Models.TaskStatus.Completed,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Tasks.AddRangeAsync(task1, task2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllTasksAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Test Task 1");
        result.Should().Contain(t => t.Title == "Test Task 2");
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnCorrectTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Specific Task",
            Priority = TaskPriority.Medium,
            Status = Models.TaskStatus.InProgress,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTaskByIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.Title.Should().Be("Specific Task");
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldAddNewTask()
    {
        // Arrange
        var newTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "New Task",
            Description = "Test Description",
            Priority = TaskPriority.Critical,
            Status = Models.TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _service.CreateTaskAsync(newTask);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(newTask.Id);
        
        var savedTask = await _context.Tasks.FindAsync(newTask.Id);
        savedTask.Should().NotBeNull();
        savedTask!.Title.Should().Be("New Task");
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateExistingTask()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Original Title",
            Priority = TaskPriority.Low,
            Status = Models.TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        task.Title = "Updated Title";
        task.Status = Models.TaskStatus.Completed;

        // Act
        await _service.UpdateTaskAsync(task);

        // Assert
        var updatedTask = await _context.Tasks.FindAsync(task.Id);
        updatedTask.Should().NotBeNull();
        updatedTask!.Title.Should().Be("Updated Title");
        updatedTask.Status.Should().Be(Models.TaskStatus.Completed);
        updatedTask.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldRemoveTask()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task to Delete",
            Priority = TaskPriority.Medium,
            Status = Models.TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteTaskAsync(task.Id);

        // Assert
        result.Should().BeTrue();
        var deletedTask = await _context.Tasks.FindAsync(task.Id);
        deletedTask.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnFalseForNonExistentTask()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.DeleteTaskAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetStatisticsAsync_ShouldReturnCorrectCounts()
    {
        // Arrange
        await _context.Tasks.AddRangeAsync(
            new TaskItem { Id = Guid.NewGuid(), Title = "T1", Priority = TaskPriority.High, Status = Models.TaskStatus.Pending, CreatedAt = DateTime.UtcNow },
            new TaskItem { Id = Guid.NewGuid(), Title = "T2", Priority = TaskPriority.High, Status = Models.TaskStatus.InProgress, CreatedAt = DateTime.UtcNow },
            new TaskItem { Id = Guid.NewGuid(), Title = "T3", Priority = TaskPriority.Low, Status = Models.TaskStatus.Completed, CreatedAt = DateTime.UtcNow },
            new TaskItem { Id = Guid.NewGuid(), Title = "T4", Priority = TaskPriority.Medium, Status = Models.TaskStatus.Completed, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var stats = await _service.GetStatisticsAsync();

        // Assert
        stats.TotalTasks.Should().Be(4);
        stats.PendingTasks.Should().Be(1);
        stats.InProgressTasks.Should().Be(1);
        stats.CompletedTasks.Should().Be(2);
        stats.TasksByPriority[TaskPriority.High].Should().Be(2);
        stats.TasksByPriority[TaskPriority.Medium].Should().Be(1);
        stats.TasksByPriority[TaskPriority.Low].Should().Be(1);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
