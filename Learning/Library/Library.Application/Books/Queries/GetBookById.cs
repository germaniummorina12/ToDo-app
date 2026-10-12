using Library.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Books.Queries;

public sealed record GetBookByIdQuery(int Id) : IQuery<BookSummaryDto?>;

internal sealed class GetBookByIdHandler(ILibraryDbContext db) : IQueryHandler<GetBookByIdQuery, BookSummaryDto?>
{
    public Task<BookSummaryDto?> HandleAsync(GetBookByIdQuery query, CancellationToken ct = default) =>
        db.Books.AsNoTracking()
            .Where(b => b.Id == query.Id)
            .Select(b => new BookSummaryDto(
                b.Id, b.Title, b.Year, b.Author.Name, b.Genres.OrderBy(g => g.Name).Select(g => g.Name).ToList()))
            .FirstOrDefaultAsync(ct);
}