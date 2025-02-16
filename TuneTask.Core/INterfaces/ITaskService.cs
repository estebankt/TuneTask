using TuneTask.Core.Entities;

namespace TuneTask.Core.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    Task<TaskItem?> GetTaskByIdAsync(Guid id);
    Task<bool> CreateTaskAsync(TaskItem task);
    Task<bool> UpdateTaskAsync(TaskItem task);
    Task<bool> DeleteTaskAsync(Guid id);
    Task<List<TaskItem>> SearchTasksAsync(string query);
}
