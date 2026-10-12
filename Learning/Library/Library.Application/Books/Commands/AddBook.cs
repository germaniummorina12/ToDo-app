using System.ComponentModel.DataAnnotations;
using Library.Application.Abstractions;
using Library.Application.Common;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Books.Commands;

public sealed record AddBookCommand(
    [Required, StringLength(200)] string Title,
    [Range(1450, 2100)] int Year,
    [Range(1, int.MaxValue)] int AuthorId,
    List<int>? GenreIds) : ICommand<int>;

internal sealed class AddBookHandler(ILibraryDbContext db) : ICommandHandler<AddBookCommand, int>
{
    public async Task<int> HandleAsync(AddBookCommand command, CancellationToken ct = default)
    {
        var author = await db.Authors.FindAsync([command.AuthorId], ct)
                     ?? throw new NotFoundException(nameof(Author), command.AuthorId);

        var genreIds = command.GenreIds ?? [];
        var genres = await db.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync(ct);

        var book = new Book { Title = command.Title.Trim(), Year = command.Year, Author = author, Genres = genres };
        db.Books.Add(book);
        await db.SaveChangesAsync(ct);

        return book.Id;
    }
}