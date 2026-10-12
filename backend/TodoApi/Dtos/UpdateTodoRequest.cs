using System.ComponentModel.DataAnnotations;

namespace TodoApi.Dtos;

public class UpdateTodoRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}