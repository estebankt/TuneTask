using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneTask.Core.Entities;
using TuneTask.Core.Services;
using System.Security.Claims;

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
    /// Retrieves all tasks.
    /// </summary>
    /// <returns>A list of all tasks.</returns>
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    /// <summary>
    /// Retrieves a specific task by ID.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <returns>The requested task.</returns>
    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(Guid taskId)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null) return NotFound(new { message = "Task not found." });
        return Ok(task);
    }

    /// <summary>
    /// Creates a new task for the authenticated user.
    /// </summary>
    /// <param name="task">The task details.</param>
    /// <returns>The newly created task.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
    {
        if (task == null) return BadRequest(new { message = "Task data is required." });

        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized(new { message = "User ID not found in token." });

        task.UserId = Guid.Parse(userIdClaim.Value);

        var success = await _taskService.CreateTaskAsync(task);
        if (!success) return BadRequest(new { message = "Failed to create task." });

        return CreatedAtAction(nameof(GetTaskById), new { taskId = task.Id }, task);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="taskId">The ID of the task to update.</param>
    /// <param name="task">The updated task details.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("update/{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskItem task)
    {
        if (task == null) return BadRequest(new { message = "Task data is required." });
        if (taskId != task.Id) return BadRequest(new { message = "Task ID mismatch." });

        var success = await _taskService.UpdateTaskAsync(task);
        if (!success) return NotFound(new { message = "Task not found." });

        return NoContent();
    }

    /// <summary>
    /// Deletes a task by ID.
    /// </summary>
    /// <param name="taskId">The ID of the task to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("delete/{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var success = await _taskService.DeleteTaskAsync(taskId);
        if (!success) return NotFound(new { message = "Task not found." });

        return NoContent();
    }

    /// <summary>
    /// Searches tasks by query using AI-powered embeddings.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>A list of related tasks.</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchTasksByQuery([FromQuery] string query)
    {
        var results = await _taskService.SearchTasksAsync(query);
        return Ok(results);
    }
}
