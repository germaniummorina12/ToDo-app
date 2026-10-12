using TodoApi.Models;

namespace TodoApi.Dtos;

public static class TodoMappings
{
    public static TodoDto ToDto(this TodoItem item) =>
        new(item.Id, item.Title, item.IsCompleted, item.CreatedAt, item.CompletedAt);
}