using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService(AppDbContext db, TimeProvider clock) : ITodoService
{
    private const int MaxTitleLength = 200;

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken ct = default) =>
        await db.Todos.AsNoTracking().OrderBy(t => t.Id).ToListAsync(ct);

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<TodoItem> CreateAsync(string title, CancellationToken ct = default)
    {
        var item = new TodoItem
        {
            Title = NormalizeTitle(title),
            CreatedAt = clock.GetUtcNow().UtcDateTime
        };

        db.Todos.Add(item);
        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<TodoItem?> UpdateAsync(int id, string title, bool isCompleted, CancellationToken ct = default)
    {
        var item = await db.Todos.FindAsync([id], ct);
        if (item is null)
        {
            return null;
        }

        item.Title = NormalizeTitle(title);

        if (isCompleted && !item.IsCompleted)
        {
            item.CompletedAt = clock.GetUtcNow().UtcDateTime;
        }
        else if (!isCompleted)
        {
            item.CompletedAt = null;
        }
        item.IsCompleted = isCompleted;

        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var item = await db.Todos.FindAsync([id], ct);
        if (item is null)
        {
            return false;
        }

        db.Todos.Remove(item);
        await db.SaveChangesAsync(ct);
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