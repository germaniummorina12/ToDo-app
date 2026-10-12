using Library.Application.Abstractions;
using Library.Application.Common;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Loans.Queries;

public sealed record GetMemberLoansQuery(int MemberId, bool OpenOnly) : IQuery<IReadOnlyList<LoanDto>>;

internal sealed class GetMemberLoansHandler(ILibraryDbContext db) : IQueryHandler<GetMemberLoansQuery, IReadOnlyList<LoanDto>>
{
    public async Task<IReadOnlyList<LoanDto>> HandleAsync(GetMemberLoansQuery query, CancellationToken ct = default)
    {
        if (!await db.Members.AnyAsync(m => m.Id == query.MemberId, ct))
        {
            throw new NotFoundException(nameof(Member), query.MemberId);
        }

        var loans = db.Loans.AsNoTracking().Where(l => l.MemberId == query.MemberId);
        if (query.OpenOnly)
        {
            loans = loans.Where(l => l.ReturnedAtUtc == null);
        }

        return await loans
            .OrderByDescending(l => l.BorrowedAtUtc)
            .Select(l => new LoanDto(l.Id, l.MemberId, l.Member.Name, l.BookId, l.Book.Title, l.BorrowedAtUtc, l.ReturnedAtUtc))
            .ToListAsync(ct);
    }
}