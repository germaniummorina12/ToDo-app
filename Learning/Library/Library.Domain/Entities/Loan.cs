using Library.Domain.Exceptions;

namespace Library.Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime BorrowedAtUtc { get; set; }
    public DateTime? ReturnedAtUtc { get; set; }

    public void Return(DateTime nowUtc)
    {
        if(ReturnedAtUtc is not null)
        {
            throw new DomainException("This loan has already been returned.");
        }

        ReturnedAtUtc = nowUtc;
    }
}