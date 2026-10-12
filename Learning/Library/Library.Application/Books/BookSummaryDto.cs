namespace Library.Application.Books;

public sealed record BookSummaryDto(int Id, string Title, int Year, string Author, IReadOnlyList<string> Genres);