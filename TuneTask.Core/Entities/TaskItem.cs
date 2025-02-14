namespace TuneTask.Core.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } // Link task to a user
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
}

// Enum for task status
public enum TaskStatus
{
    Pending,
    InProgress,
    Completed
}
