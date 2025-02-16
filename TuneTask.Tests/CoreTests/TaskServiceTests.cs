using Moq;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Core.Services;
using Xunit;

namespace TuneTask.Tests.CoreTests
{
    public class TaskServiceTests
    {
        private readonly Mock<IRepository<TaskItem>> _mockTaskRepository;
        private readonly Mock<IAIService> _mockAIService;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockTaskRepository = new Mock<IRepository<TaskItem>>();
            _mockAIService = new Mock<IAIService>();

            _taskService = new TaskService(_mockTaskRepository.Object, _mockAIService.Object);
        }

        [Fact]
        public async Task SearchTasksAsync_ShouldReturnRelatedTasks()
        {
            // Arrange
            var query = "Deep Focus";
            var queryEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
            var storedTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Deep Focus Coding",
                Description = "A long coding session with no distractions.",
                Embedding = new float[] { 0.11f, 0.21f, 0.29f }
            };

            _mockAIService.Setup(ai => ai.GenerateTaskEmbeddingAsync(query))
                .ReturnsAsync(queryEmbedding);

            _mockTaskRepository.Setup(repo => repo.SearchTasksAsync(queryEmbedding, 5))
                .ReturnsAsync(new List<TaskItem> { storedTask });

            // Act
            var result = await _taskService.SearchTasksAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Deep Focus Coding", result[0].Title);
        }
    }
}
