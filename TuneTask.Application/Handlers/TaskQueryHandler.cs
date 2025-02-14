namespace TuneTask.Application.Commands;

public class CreateTaskCommand
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
