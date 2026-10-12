using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Abstractions;

public interface ILibraryDbContext
{
    DbSet<Author> Authors { get; }
    DbSet<Book> Books { get; }
    DbSet<Genre> Genres { get; }
    DbSet<Member> Members { get; }
    DbSet<Loan> Loans { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}