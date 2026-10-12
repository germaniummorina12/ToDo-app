using Library.Application.Abstractions;
using Library.Application.Books;
using Library.Application.Books.Commands;
using Library.Application.Books.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookSummaryDto>>> GetAll(
        [FromQuery] string? genre,
        [FromServices] IQueryHandler<GetBooksQuery, IReadOnlyList<BookSummaryDto>> handler,
        CancellationToken ct) =>
        Ok(await handler.HandleAsync(new GetBooksQuery(genre), ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookSummaryDto>> GetById(
        int id,
        [FromServices] IQueryHandler<GetBookByIdQuery, BookSummaryDto?> handler,
        CancellationToken ct)
    {
        var book = await handler.HandleAsync(new GetBookByIdQuery(id), ct);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        AddBookCommand command,
        [FromServices] ICommandHandler<AddBookCommand, int> handler,
        CancellationToken ct)
    {
        var id = await handler.HandleAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}