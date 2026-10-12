using Library.Application.Abstractions;
using Library.Application.Loans;
using Library.Application.Loans.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/loans")]
public sealed class LoansController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<LoanDto>> Borrow(
        BorrowBookCommand command,
        [FromServices] ICommandHandler<BorrowBookCommand, LoanDto> handler,
        CancellationToken ct)
    {
        var loan = await handler.HandleAsync(command, ct);
        return StatusCode(StatusCodes.Status201Created, loan);
    }

    [HttpPost("{id:int}/return")]
    public async Task<ActionResult<LoanDto>> Return(
        int id,
        [FromServices] ICommandHandler<ReturnBookCommand, LoanDto> handler,
        CancellationToken ct) =>
        Ok(await handler.HandleAsync(new ReturnBookCommand(id), ct));
}