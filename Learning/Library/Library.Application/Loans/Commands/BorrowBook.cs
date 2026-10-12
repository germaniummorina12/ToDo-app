using System.ComponentModel.DataAnnotations;
using Library.Application.Abstractions;
using Library.Application.Common;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Library.Application.Loans.Commands;

public sealed record BorrowBookCommand([Range(1, int.MaxValue)] int MemberId, [Range(1, int.MaxValue)] int BookId)
    : ICommand<LoanDto>;

internal sealed class BorrowBookHandler(ILibraryDbContext db, IOptions<LibraryOptions> options, TimeProvider clock)
    : ICommandHandler<BorrowBookCommand, LoanDto>
{
    public async Task<LoanDto> HandleAsync(BorrowBookCommand command, CancellationToken ct = default)
    {
        var member = await db.Members.Include(m => m.Loans).FirstOrDefaultAsync(m => m.Id == command.MemberId, ct)
                     ?? throw new NotFoundException(nameof(Member), command.MemberId);
        var book = await db.Books.Include(b => b.Loans).FirstOrDefaultAsync(b => b.Id == command.BookId, ct)
                   ?? throw new NotFoundException(nameof(Book), command.BookId);
        var loan = member.Borrow(book, options.Value.MaxLoansPerMember, clock.GetUtcNow().UtcDateTime);
        await db.SaveChangesAsync(ct);

        return new LoanDto(loan.Id, member.Id, member.Name, book.Id, book.Title, loan.BorrowedAtUtc, loan.ReturnedAtUtc);
    }
}