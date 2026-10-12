using System.ComponentModel.DataAnnotations;

namespace TodoApi.Dtos;

public class CreateTodoRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
}