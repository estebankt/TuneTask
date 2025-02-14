using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;

namespace TuneTask.Core.Services;

public class TaskService
{
    private readonly IRepository<TaskItem> _taskRepository;

    public TaskService(IRepository<TaskItem> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
    {
        return await _taskRepository.GetByIdAsync(id);
    }

    public async Task<bool> CreateTaskAsync(TaskItem task)
    {
        // Basic Validation
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Task title cannot be empty.");

        task.Id = Guid.NewGuid(); // Ensure unique ID
        task.CreatedAt = DateTime.UtcNow;
        task.Status = Entities.TaskStatus.Pending;

        return await _taskRepository.AddAsync(task);
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        var existingTask = await _taskRepository.GetByIdAsync(task.Id);
        if (existingTask == null)
            throw new KeyNotFoundException("Task not found.");

        return await _taskRepository.UpdateAsync(task);
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask == null)
            throw new KeyNotFoundException("Task not found.");

        return await _taskRepository.DeleteAsync(id);
    }
}
