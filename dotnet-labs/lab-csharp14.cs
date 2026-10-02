var person = new Person();
person.Name = "  Ada  ";
Console.WriteLine($"[{person.Name}]");

Customer? nobody = null;
nobody?.Nickname = "ghost";
Console.WriteLine(nobody is null);

Console.WriteLine("hello world".IsBlank);
Console.WriteLine("   ".IsBlank);
Console.WriteLine("A very long sentence".Truncate(6));

Console.WriteLine(nameof(List<>));

class Person
{
    public string Name
    {
        get;
        set => field = value.Trim();
    } = string.Empty;
}

class Customer
{
    public string? Nickname { get; set; }
}

static class StringExtensions
{
    extension(string text)
    {
        public bool IsBlank => string.IsNullOrWhiteSpace(text);
        public string Truncate(int max) => text.Length <= max ? text : text[..max];
    }
}