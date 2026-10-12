using Library.Application.Abstractions;
using Library.Application.Common;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Loans.Commands;

public sealed record ReturnBookCommand(int LoanId) : ICommand<LoanDto>;

internal sealed class ReturnBookHandler(ILibraryDbContext db, TimeProvider clock) : ICommandHandler<ReturnBookCommand, LoanDto>
{
    public async Task<LoanDto> HandleAsync(ReturnBookCommand command, CancellationToken ct = default)
    {
        var loan = await db.Loans
                       .Include(l => l.Member)
                       .Include(l => l.Book)
                       .FirstOrDefaultAsync(l => l.Id == command.LoanId, ct)
                   ?? throw new NotFoundException(nameof(Loan), command.LoanId);

        loan.Return(clock.GetUtcNow().UtcDateTime);
        await db.SaveChangesAsync(ct);

        return new LoanDto(loan.Id, loan.MemberId, loan.Member.Name, loan.BookId, loan.Book.Title,
            loan.BorrowedAtUtc, loan.ReturnedAtUtc);
    }
}