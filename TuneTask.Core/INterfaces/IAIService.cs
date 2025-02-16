namespace TuneTask.Core.Interfaces;

public interface IAIService
{
    Task<float[]> GenerateTaskEmbeddingAsync(string taskDescription);
}
