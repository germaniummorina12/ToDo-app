using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken ct = default);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TodoItem> CreateAsync(string title, CancellationToken ct = default);
    Task<TodoItem?> UpdateAsync(int id, string title, bool isCompleted, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}