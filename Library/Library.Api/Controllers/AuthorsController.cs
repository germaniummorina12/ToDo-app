using Library.Api.Data;
using Library.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/authors")]
public sealed class AuthorsController(LibraryDbContext db) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDetailsDto>> Get(int id, CancellationToken ct)
    {
        var author = await db.Authors.AsNoTracking()
            .Include(a => a.Profile)
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        if (author is null)
        {
            return NotFound();
        }

        return Ok(new AuthorDetailsDto(
            author.Id,
            author.Name,
            author.Profile?.Biography,
            author.Books.Count,
            author.Books.OrderBy(b => b.Year).Select(b => b.Title).ToList()));
    }
}