using Microsoft.AspNetCore.Mvc;
using SecureTaskApi.Models;
using SecureTaskApi.Services;

namespace SecureTaskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks()
    {
        _logger.LogInformation("Fetching all tasks");
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(Guid id)
    {
        _logger.LogInformation("Fetching task with ID: {TaskId}", id);
        var task = await _taskService.GetTaskByIdAsync(id);
        
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        _logger.LogInformation("Creating new task: {TaskTitle}", createTaskDto.Title);
        
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            Priority = createTaskDto.Priority,
            Status = Models.TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var createdTask = await _taskService.CreateTaskAsync(task);
        return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);
        
        var existingTask = await _taskService.GetTaskByIdAsync(id);
        if (existingTask == null)
        {
            return NotFound();
        }

        existingTask.Title = updateTaskDto.Title ?? existingTask.Title;
        existingTask.Description = updateTaskDto.Description ?? existingTask.Description;
        existingTask.Priority = updateTaskDto.Priority;
        existingTask.Status = updateTaskDto.Status;
        existingTask.UpdatedAt = DateTime.UtcNow;

        await _taskService.UpdateTaskAsync(existingTask);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);
        
        var result = await _taskService.DeleteTaskAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<TaskStatistics>> GetStatistics()
    {
        _logger.LogInformation("Fetching task statistics");
        var stats = await _taskService.GetStatisticsAsync();
        return Ok(stats);
    }
}
