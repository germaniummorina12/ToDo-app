using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(int id);
    Task<TodoItem> CreateAsync(string title);
    Task<TodoItem?> UpdateAsync(int id, string title, bool isCompleted);
    Task<bool> DeleteAsync(int id);
}