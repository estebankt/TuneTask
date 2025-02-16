using TuneTask.Core.Entities;

namespace TuneTask.Core.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<bool> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<List<T>> SearchTasksAsync(float[] queryEmbedding, int topN = 5);
}