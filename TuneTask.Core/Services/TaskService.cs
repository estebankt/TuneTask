using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Shared.Exceptions;

namespace TuneTask.Core.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IAIService _aiService;

    public TaskService(IRepository<TaskItem> taskRepository, IAIService aiService)
    {
        _taskRepository = taskRepository;
        _aiService = aiService;
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task<TaskItem> GetTaskByIdAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new TaskNotFoundException(taskId); 

        return task;
    }

    public async Task<bool> CreateTaskAsync(TaskItem task)
    {
        // Basic Validation
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Task title cannot be empty.");

        task.Id = Guid.NewGuid(); // Ensure unique ID
        task.CreatedAt = DateTime.UtcNow;
        task.Status = Entities.TaskStatus.Pending;
        task.Embedding = await _aiService.GenerateTaskEmbeddingAsync(task.Description);

        return await _taskRepository.AddAsync(task);
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        var existingTask = await _taskRepository.GetByIdAsync(task.Id);
        if (existingTask == null)
            throw new KeyNotFoundException("Task not found.");

        if (existingTask.Description != task.Description)
        {
            task.Embedding = await _aiService.GenerateTaskEmbeddingAsync(task.Description);
        }
        else
        {
            task.Embedding = existingTask.Embedding;
        }

        return await _taskRepository.UpdateAsync(task);
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask == null)
            throw new KeyNotFoundException("Task not found.");

        return await _taskRepository.DeleteAsync(id);
    }

    public async Task<List<TaskItem>> SearchTasksAsync(string query)
    {
        var queryEmbedding = await _aiService.GenerateTaskEmbeddingAsync(query);
        return await _taskRepository.SearchTasksAsync(queryEmbedding);
    }
}
