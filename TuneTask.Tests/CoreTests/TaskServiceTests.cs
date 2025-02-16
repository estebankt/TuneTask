using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Core.Services;
using TuneTask.Shared.Exceptions;
using Xunit;

namespace TuneTask.Core.Services.Tests
{
    public class TaskServiceTest
    {
        private readonly Mock<IRepository<TaskItem>> _taskRepositoryMock;
        private readonly Mock<IAIService> _aiServiceMock;
        private readonly TaskService _taskService;

        public TaskServiceTest()
        {
            _taskRepositoryMock = new Mock<IRepository<TaskItem>>();
            _aiServiceMock = new Mock<IAIService>();
            _taskService = new TaskService(_taskRepositoryMock.Object, _aiServiceMock.Object);
        }

        [Fact]
        public async Task GetAllTasksAsync_ReturnsAllTasks()
        {
            // Arrange
            var tasks = new List<TaskItem> { new TaskItem { Id = Guid.NewGuid(), Title = "Test Task" } };
            _taskRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksAsync();

            // Assert
            Assert.Equal(tasks, result);
        }

        [Fact]
        public async Task GetTaskByIdAsync_TaskExists_ReturnsTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Title = "Test Task" };
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.Equal(task, result);
        }

        [Fact]
        public async Task GetTaskByIdAsync_TaskDoesNotExist_ThrowsTaskNotFoundException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync((TaskItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<TaskNotFoundException>(() => _taskService.GetTaskByIdAsync(taskId));
        }

        [Fact]
        public async Task CreateTaskAsync_ValidTask_ReturnsTrue()
        {
            // Arrange
            var task = new TaskItem { Title = "Test Task", Description = "Test Description" };
            _aiServiceMock.Setup(ai => ai.GenerateTaskEmbeddingAsync(task.Description)).ReturnsAsync(new float[0]);
            _taskRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync(true);

            // Act
            var result = await _taskService.CreateTaskAsync(task);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateTaskAsync_EmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var task = new TaskItem { Title = "", Description = "Test Description" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _taskService.CreateTaskAsync(task));
        }

        [Fact]
        public async Task UpdateTaskAsync_TaskExists_ReturnsTrue()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Updated Task", Description = "Updated Description" };
        }
    }
}