namespace TodoApi.Dtos;

public sealed record TodoDto(int Id, string Title, bool IsCompleted, DateTime CreatedAt, DateTime? CompletedAt);