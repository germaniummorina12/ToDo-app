using Library.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data;

public static class DbSeeder
{
    // Adds sample rows to an EMPTY development database. The tables themselves come from the migrations.
    public static async Task SeedAsync(LibraryDbContext db)
    {
        if (await db.Authors.AnyAsync())
        {
            return;
        }

        var dystopia = new Genre { Name = "Dystopia" };
        var classic = new Genre { Name = "Classic" };
        var fantasy = new Genre { Name = "Fantasy" };

        var orwell = new Author
        {
            Name = "George Orwell",
            Profile = new AuthorProfile { Biography = "English novelist and essayist." },
            Books =
            [
                new Book { Title = "Nineteen Eighty-Four", Year = 1949, Genres = [dystopia, classic] },
                new Book { Title = "Animal Farm", Year = 1945, Genres = [classic] }
            ]
        };

        var tolkien = new Author
        {
            Name = "J. R. R. Tolkien",
            Books = [new Book { Title = "The Hobbit", Year = 1937, Genres = [fantasy, classic] }]
        };

        db.Authors.AddRange(orwell, tolkien);
        db.Members.AddRange(
            new Member { Name = "Ada Lovelace", Email = "ada@example.com" },
            new Member { Name = "Alan Turing", Email = "alan@example.com" });

        await db.SaveChangesAsync();      // one call saves the whole graph: authors, profile, books, genres, links
    }
}