namespace Library.Api.Entities;

public class AuthorProfile
{
    public int Id { get; set; }
    public string? Biography { get; set; }
    public string? Website { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}