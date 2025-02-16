using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TuneTask.API.Controllers;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using Xunit;

namespace TuneTask.Tests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly TaskController _taskController;

        public TaskControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _taskController = new TaskController(_taskServiceMock.Object);
        }

        #region GetAllTasks

        [Fact]
        public async Task GetAllTasks_ReturnsOkResult_WithTaskList()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", Description = "Description 1", Status = Core.Entities.TaskStatus.Pending },
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", Description = "Description 2", Status = Core.Entities.TaskStatus.Completed }
            };

            _taskServiceMock.Setup(service => service.GetAllTasksAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _taskController.GetAllTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTasks = Assert.IsType<List<TaskItem>>(okResult.Value);
            Assert.Equal(2, returnedTasks.Count);
        }

        #endregion

        #region GetTaskById

        [Fact]
        public async Task GetTaskById_ValidId_ReturnsOkResult_WithTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Title = "Sample Task", Description = "Task Description", Status = Core.Entities.TaskStatus.InProgress };

            _taskServiceMock.Setup(service => service.GetTaskByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _taskController.GetTaskById(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTask = Assert.IsType<TaskItem>(okResult.Value);
            Assert.Equal(taskId, returnedTask.Id);
        }

        [Fact]
        public async Task GetTaskById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskServiceMock.Setup(service => service.GetTaskByIdAsync(taskId)).ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskController.GetTaskById(taskId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region CreateTask

        [Fact]
        public async Task CreateTask_ValidData_ReturnsCreatedResult()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "New Task", Description = "Task Description", Status = Core.Entities.TaskStatus.Pending };

            _taskServiceMock.Setup(service => service.CreateTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync(true);

            // Mock user identity
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);
            _taskController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            // Act
            var result = await _taskController.CreateTask(task);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedTask = Assert.IsType<TaskItem>(createdResult.Value);
            Assert.Equal(task.Title, returnedTask.Title);
        }

        [Fact]
        public async Task CreateTask_NoUserId_ReturnsUnauthorized()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "New Task", Description = "Task Description", Status = Core.Entities.TaskStatus.Pending };

            // No User Identity
            _taskController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            // Act
            var result = await _taskController.CreateTask(task);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateTask_Fails_ReturnsBadRequest()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "New Task", Description = "Task Description", Status = Core.Entities.TaskStatus.Pending };

            _taskServiceMock.Setup(service => service.CreateTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync(false);

            // Mock user identity
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);
            _taskController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            // Act
            var result = await _taskController.CreateTask(task);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region UpdateTask

        [Fact]
        public async Task UpdateTask_ValidData_ReturnsNoContent()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Title = "Updated Task", Description = "Updated Description", Status = Core.Entities.TaskStatus.InProgress };

            _taskServiceMock.Setup(service => service.UpdateTaskAsync(task)).ReturnsAsync(true);

            // Act
            var result = await _taskController.UpdateTask(taskId, task);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateTask_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Updated Task", Description = "Updated Description", Status = Core.Entities.TaskStatus.InProgress };

            // Act
            var result = await _taskController.UpdateTask(taskId, task);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateTask_NotFound_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Title = "Updated Task", Description = "Updated Description", Status = Core.Entities.TaskStatus.InProgress };

            _taskServiceMock.Setup(service => service.UpdateTaskAsync(task)).ReturnsAsync(false);

            // Act
            var result = await _taskController.UpdateTask(taskId, task);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region DeleteTask

        [Fact]
        public async Task DeleteTask_ValidId_ReturnsNoContent()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskServiceMock.Setup(service => service.DeleteTaskAsync(taskId)).ReturnsAsync(true);

            // Act
            var result = await _taskController.DeleteTask(taskId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTask_NotFound_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskServiceMock.Setup(service => service.DeleteTaskAsync(taskId)).ReturnsAsync(false);

            // Act
            var result = await _taskController.DeleteTask(taskId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region SearchTasksByQuery

        [Fact]
        public async Task SearchTasksByQuery_ReturnsOkResult_WithTasks()
        {
            // Arrange
            var query = "test";
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Test Task", Description = "Description", Status = Core.Entities.TaskStatus.Pending }
            };

            _taskServiceMock.Setup(service => service.SearchTasksAsync(query)).ReturnsAsync(tasks);

            // Act
            var result = await _taskController.SearchTasksByQuery(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTasks = Assert.IsType<List<TaskItem>>(okResult.Value);
            Assert.Single(returnedTasks);
        }

        #endregion
    }
}
