using Microsoft.EntityFrameworkCore;
using SecureTaskApi.Data;
using SecureTaskApi.Models;

namespace SecureTaskApi.Services;

public class TaskService : ITaskService
{
    private readonly TaskDbContext _context;
    private readonly ILogger<TaskService> _logger;

    public TaskService(TaskDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        _logger.LogDebug("Retrieving all tasks from database");
        return await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
    {
        _logger.LogDebug("Retrieving task with ID: {TaskId}", id);
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        _logger.LogInformation("Creating new task: {TaskTitle}", task.Title);
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
    {
        _logger.LogInformation("Updating task: {TaskId}", task.Id);
        
        if (task.Status == TaskStatus.Completed && task.CompletedAt == null)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        
        _context.Entry(task).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        _logger.LogInformation("Deleting task: {TaskId}", id);
        var task = await _context.Tasks.FindAsync(id);
        
        if (task == null)
        {
            return false;
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TaskStatistics> GetStatisticsAsync()
    {
        _logger.LogDebug("Calculating task statistics");
        
        var tasks = await _context.Tasks.ToListAsync();
        
        var stats = new TaskStatistics
        {
            TotalTasks = tasks.Count,
            PendingTasks = tasks.Count(t => t.Status == TaskStatus.Pending),
            InProgressTasks = tasks.Count(t => t.Status == TaskStatus.InProgress),
            CompletedTasks = tasks.Count(t => t.Status == TaskStatus.Completed),
            CancelledTasks = tasks.Count(t => t.Status == TaskStatus.Cancelled),
            TasksByPriority = tasks.GroupBy(t => t.Priority)
                                  .ToDictionary(g => g.Key, g => g.Count())
        };

        return stats;
    }
}
