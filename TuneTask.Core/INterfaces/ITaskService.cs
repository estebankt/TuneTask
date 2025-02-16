using TuneTask.Core.Entities;

namespace TuneTask.Core.Interfaces;

public interface ITaskService
{
    Task<TaskItem> CreateTaskAsync(TaskItem task);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<List<TaskItem>> SearchTasksAsync(string query);
}
