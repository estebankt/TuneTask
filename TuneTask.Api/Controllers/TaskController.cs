using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneTask.Core.Entities;
using TuneTask.Core.Services;

namespace TuneTask.API.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Get all tasks.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    /// <summary>
    /// Get a task by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null) return NotFound("Task not found.");
        return Ok(task);
    }

    /// <summary>
    /// Create a new task.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskItem task)
    {
        if (task == null) return BadRequest("Task data is required.");

        var success = await _taskService.CreateTaskAsync(task);
        if (!success) return BadRequest("Failed to create task.");

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    /// <summary>
    /// Update an existing task.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaskItem task)
    {
        if (task == null) return BadRequest("Task data is required.");
        if (id != task.Id) return BadRequest("Task ID mismatch.");

        var success = await _taskService.UpdateTaskAsync(task);
        if (!success) return NotFound("Task not found.");

        return NoContent();
    }

    /// <summary>
    /// Delete a task by ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _taskService.DeleteTaskAsync(id);
        if (!success) return NotFound("Task not found.");

        return NoContent();
    }
}
