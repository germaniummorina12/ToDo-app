namespace Library.Api.Options;

public sealed class LibraryOptions
{
    public const string SectionName = "Library";

    public string Name { get; init; } = "Library";
    public int MaxLoansPerMember { get; init; } = 5;
}