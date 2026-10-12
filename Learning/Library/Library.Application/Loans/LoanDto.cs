namespace Library.Application.Loans;

public sealed record LoanDto(
    int Id, int MemberId, string Member, int BookId, string Book, DateTime BorrowedAtUtc, DateTime? ReturnedAtUtc);