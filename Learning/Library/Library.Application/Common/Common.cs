namespace Library.Application.Common;

public sealed class LibraryOptions
{
    public const string SectionName = "Library";

    public string Name { get; init; } = "Library";
    public int MaxLoansPerMember { get; init; } = 5;
}

public sealed class NotFoundException(string entity, object key) : Exception($"{entity} '{key}' was not found.");