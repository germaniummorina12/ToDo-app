using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private const int MaxTitleLength = 200;
    private readonly AppDbContext _db;

    public TodoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync()
    {
        return await _db.Todos
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        return await _db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TodoItem> CreateAsync(string title)
    {
        var item = new TodoItem
        {
            Title = NormalizeTitle(title),
            CreatedAt = DateTime.UtcNow
        };

        _db.Todos.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<TodoItem?> UpdateAsync(int id, string title, bool isCompleted)
    {
        var item = await _db.Todos.FindAsync(id);
        if (item is null)
        {
            return null;
        }

        item.Title = NormalizeTitle(title);

        if (isCompleted && !item.IsCompleted)
        {
            item.CompletedAt = DateTime.UtcNow;
        }
        else if (!isCompleted)
        {
            item.CompletedAt = null;
        }
        item.IsCompleted = isCompleted;

        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.Todos.FindAsync(id);
        if (item is null)
        {
            return false;
        }

        _db.Todos.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    private static string NormalizeTitle(string? title)
    {
        var cleaned = title?.Trim();

        if (string.IsNullOrEmpty(cleaned))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }
        if (cleaned.Length > MaxTitleLength)
        {
            throw new ArgumentException($"Title cannot exceed {MaxTitleLength} characters.", nameof(title));
        }

        return cleaned;
    }
}