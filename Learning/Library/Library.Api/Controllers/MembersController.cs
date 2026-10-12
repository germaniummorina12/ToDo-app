using Library.Application.Abstractions;
using Library.Application.Loans;
using Library.Application.Loans.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/members")]
public sealed class MembersController : ControllerBase
{
    [HttpGet("{id:int}/loans")]
    public async Task<ActionResult<IReadOnlyList<LoanDto>>> GetLoans(
        int id,
        [FromQuery] bool openOnly,
        [FromServices] IQueryHandler<GetMemberLoansQuery, IReadOnlyList<LoanDto>> handler,
        CancellationToken ct) =>
        Ok(await handler.HandleAsync(new GetMemberLoansQuery(id, openOnly), ct));
}