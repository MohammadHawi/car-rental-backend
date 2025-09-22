using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TasksController> _logger;
    
    public TasksController(ITaskRepository taskRepository, ILogger<TasksController> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }
    
    // GET: api/Tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
    {
        try
        {
            // Update overdue tasks first
            await _taskRepository.UpdateOverdueTasksAsync();
            
            var tasks = await _taskRepository.GetAllTasksAsync();
            return Ok(tasks.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tasks");
            return StatusCode(500, "Internal server error");
        }
    }
    
    // GET: api/Tasks/active
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetActiveTasks()
    {
        try
        {
            // Update overdue tasks first
            await _taskRepository.UpdateOverdueTasksAsync();
            
            var tasks = await _taskRepository.GetActiveTasksAsync();
            return Ok(tasks.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active tasks");
            return StatusCode(500, "Internal server error");
        }
    }
    
    // GET: api/Tasks/completed
    [HttpGet("completed")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetCompletedTasks()
    {
        try
        {
            var tasks = await _taskRepository.GetCompletedTasksAsync();
            return Ok(tasks.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completed tasks");
            return StatusCode(500, "Internal server error");
        }
    }
    
    // GET: api/Tasks/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        try
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            
            if (task == null)
            {
                return NotFound();
            }
            
            return MapToDto(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting task with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
    
    // POST: api/Tasks
    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createTaskDto)
    {
        try
        {
            var task = new ToDoTask
            {
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                IsImportant = createTaskDto.IsImportant,
                CreatedBy = createTaskDto.CreatedBy ?? (User?.Identity?.Name ?? "Admin")
            };
            
            var createdTask = await _taskRepository.CreateTaskAsync(task);
            
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, MapToDto(createdTask));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, "Internal server error");
        }
    }
    
    // PUT: api/Tasks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
    {
        try
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            
            if (task == null)
            {
                return NotFound();
            }
            
            task.Description = updateTaskDto.Description;
            task.DueDate = updateTaskDto.DueDate;
            task.IsImportant = updateTaskDto.IsImportant;
            
            var result = await _taskRepository.UpdateTaskAsync(task);
            
            if (result)
            {
                return NoContent();
            }
            
            return StatusCode(500, "Failed to update task");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating task with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
    
    // POST: api/Tasks/5/complete
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteTask(int id)
    {
        try
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            
            if (task == null)
            {
                return NotFound();
            }
            
            var completedBy = User.Identity.Name ?? "System";
            var result = await _taskRepository.CompleteTaskAsync(id, completedBy);
            
            if (result)
            {
                return NoContent();
            }
            
            return StatusCode(500, "Failed to complete task");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error completing task with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
    
    // DELETE: api/Tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var result = await _taskRepository.DeleteTaskAsync(id);
            
            if (result)
            {
                return NoContent();
            }
            
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting task with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
    
    private static TaskDto MapToDto(ToDoTask task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status.ToString(),
            IsImportant = task.IsImportant,
            CreatedAt = task.CreatedAt,
            CreatedBy = task.CreatedBy,
            CompletedAt = task.CompletedAt,
            CompletedBy = task.CompletedBy
        };
    }
}