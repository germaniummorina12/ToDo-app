namespace Library.Api.Entities;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public AuthorProfile? Profile { get; set; }
    public List<Book> Books { get; set; } = [];
}