using System.ComponentModel.DataAnnotations;

namespace Library.Api.Dtos;

public sealed record BookSummaryDto(int Id, string Title, int Year, string Author, IReadOnlyList<string> Genres);

public sealed record AuthorDetailsDto(int Id, string Name, string? Biography, int BookCount, IReadOnlyList<string> Titles);

public sealed record GenreCountDto(string Genre, int Books);

public sealed class CreateBookRequest
{
    [Required, StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [Range(1450, 2100)]
    public int Year { get; init; }

    [Range(1, int.MaxValue)]
    public int AuthorId { get; init; }

    public List<int> GenreIds { get; init; } = [];
}