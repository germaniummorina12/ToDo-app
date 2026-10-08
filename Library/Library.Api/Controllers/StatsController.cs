using Library.Api.Data;
using Library.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/stats")]
public sealed class StateController(LibraryDbContext db) : ControllerBase
{
    [HttpGet("books-per-genre")]
    public async Task<ActionResult<IReadOnlyList<GenreCountDto>>> BooksPerGenre(CancellationToken ct) =>
        Ok(await db.Genres.AsNoTracking()
            .OrderByDescending(g => g.Books.Count)
            .ThenBy(g => g.Name)
            .Select(g => new GenreCountDto(g.Name, g.Books.Count))
            .ToListAsync(ct));
}

