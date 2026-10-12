using Library.Domain.Exceptions;

namespace Library.Domain.Entities;

public class Member
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public List<Loan> Loans { get; set; } = [];

    public Loan Borrow(Book book, int maxOpenLoans, DateTime nowUtc)
    {
        if(book.Loans.Any(l => l.ReturnedAtUtc is null))
        {
            throw new DomainException($"'{book.Title}' is already on loan.");
        }
        var openLoans = Loans.Count(l => l.ReturnedAtUtc is null);
        if(openLoans >= maxOpenLoans)
        {
            throw new DomainException($"{Name} already has {openLoans} open loans (the limit is {maxOpenLoans})");
        }
        var loan = new Loan { Member = this, Book = book, BorrowedAtUtc = nowUtc };
        Loans.Add(loan);
        return loan;
    }
}