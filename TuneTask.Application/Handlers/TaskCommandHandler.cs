using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Application.Commands;

namespace TuneTask.Application.Handlers;

public class TaskCommandHandler
{
    private readonly IRepository<TaskItem> _taskRepository;

    public TaskCommandHandler(IRepository<TaskItem> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskItem> HandleCreateTask(CreateTaskCommand command)
    {
        var task = new TaskItem
        {
            UserId = command.UserId,
            Title = command.Title,
            Description = command.Description
        };

        await _taskRepository.AddAsync(task);
        return task;
    }
}
