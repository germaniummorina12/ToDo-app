using Library.Application.Abstractions;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Books.Queries;

public sealed record GetBooksQuery(string? Genre) : IQuery<IReadOnlyList<BookSummaryDto>>;

internal sealed class GetBooksHandler(ILibraryDbContext db) : IQueryHandler<GetBooksQuery, IReadOnlyList<BookSummaryDto>>
{
    public async Task<IReadOnlyList<BookSummaryDto>> HandleAsync(GetBooksQuery query, CancellationToken ct = default)
    {
        IQueryable<Book> books = db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Genre))
        {
            books = books.Where(b => b.Genres.Any(g => g.Name == query.Genre));
        }

        return await books
            .OrderBy(b => b.Title)
            .Select(b => new BookSummaryDto(
                b.Id, b.Title, b.Year, b.Author.Name, b.Genres.OrderBy(g => g.Name).Select(g => g.Name).ToList()))
            .ToListAsync(ct);
    }
}