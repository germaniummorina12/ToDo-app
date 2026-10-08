using Library.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Api.Data;

public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.Property(a => a.Name).IsRequired().HasMaxLength(120);

        builder.HasOne(a => a.Profile)
            .WithOne(p => p.Author)
            .HasForeignKey<AuthorProfile>(p => p.AuthorId);

        builder.HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AuthorProfileConfiguration : IEntityTypeConfiguration<AuthorProfile>
{
    public void Configure(EntityTypeBuilder<AuthorProfile> builder)
    {
        builder.Property(p => p.Biography).HasMaxLength(2000);
        builder.Property(p => p.Website).HasMaxLength(300);
    }
}

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.HasIndex(b => b.Title);

        builder.HasMany(b => b.Genres)
            .WithMany(g => g.Books)
            .UsingEntity(join => join.ToTable("BookGenres"));
    }
}

public sealed class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.Property(g => g.Name).IsRequired().HasMaxLength(60);
        builder.HasIndex(g => g.Name).IsUnique();
    }
}

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.Property(m => m.Name).IsRequired().HasMaxLength(120);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(m => m.Email).IsUnique();
    }
}

public sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasOne(l => l.Member).WithMany(m => m.Loans).HasForeignKey(l => l.MemberId);
        builder.HasOne(l => l.Book).WithMany(b => b.Loans).HasForeignKey(l => l.BookId);
        builder.HasIndex(l => new { l.MemberId, l.ReturnedAtUtc });
    }

}