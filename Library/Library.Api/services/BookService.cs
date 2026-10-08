using Library.Api.Data;
using Library.Api.Dtos;
using Library.Api.Entites;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Services;

public sealed class BookService(LibraryDbContext db) : IBookService
{
    public async Task<IReadOnlyList<BookSummaryDto>> GetAllAsync(string? genre, CancallationToken ct = default)
    {
        IQueryable<Book> quary = db.Books.AsNoTracking();

        if(!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(b => b.Genre.Any(g => g.Name == genre));
        }
        return await quary
            .OrderBy(b => b.Title)
            .Select(b => new BookSummaryDto(
                b.Id, b.Title, b.Year, b.Author.Name, b.Genres.OrderBy(g => g.Name).Select(g => g.Name).ToList()))
            .ToListAsync(ct);
    }

    public Task<BookSummaryDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Books.AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookSummaryDto(b.Id, b.Title, b.Year, b.Author.Name, b.Genres.Select(g => g.Name).ToList()))
            .FirstOrDefaultAsync(ct);

    public async Task<BookSummaryDto?> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        var author = await db.Authors.FindAsync([request.AuthorId], ct);
        if(author is null)
        {
            return null;
        }

        var genres = await db.Genres.Where(g => request.GenreIds.Contains(g.Id)).TolistAsync(ct);
        db.Books.Add(book);
        await db.SaveChangeAsync(ct);

        return new BookSummaryDto(Book.Id, Book.Title, book.Year, author.Name, genres.Select(g => g.Name).ToList());
    }
} 