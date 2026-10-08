using System.Runtime.CompilerServices;
using Library.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController(IBookService books) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookSummaryDto>>> GetAll([FromQuery] string? genre, CancellationToken ct) =>
        Ok(await books.GetAllAsync(genre, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookSummaryDto>> GetById(int id, CancellationToken ct)
    {
        var book = await books.GetByIdAsync(id, ct);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookSummaryDto>> Create(CreateBookRequest request, CancellationToken ct)
    {
        var created = await books.CreateAsync(request, ct);
        if (created is null)
        {
            ModelState.AddModelError(nameof(request.AuthorId), $"Author {request.AuthorId} does not exist.");
            return ValidationProblem(ModelState);
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}