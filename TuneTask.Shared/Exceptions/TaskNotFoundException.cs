namespace TuneTask.Shared.Exceptions;

public class TaskNotFoundException : BaseException
{
    public TaskNotFoundException(Guid taskId)
        : base($"Task with ID '{taskId}' not found.", 404) { }
}
