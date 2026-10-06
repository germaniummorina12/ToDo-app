using Library.Api.Models;

namespace Library.Api.Services;

public sealed class BookCatalog
{
    private readonly List<Book> _books = [];
    private readonly Lock _gate = new();
    private int _nextId = 1;
}

