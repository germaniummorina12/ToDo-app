var p1 = new PointStruct(1, 2);
var p2 = p1;
p2.X = 99;
Console.WriteLine($"struct: p1.X={p1.X}, p2.X={p2.X}");

var c1 = new PointClass(1, 2);
var c2 = c1;
c2.X = 99;
Console.WriteLine($"class: c1.X={c1.X}, c2.X={c2.X}");

var r1 = new PointRecord(1, 2);
var r2 = r1 with { X = 99 };
Console.WriteLine($"record: r1.X={r1.X}, r2.X={r2.X}");

Console.WriteLine(ReferenceEquals(c1, c2));
Console.WriteLine(new PointClass(1, 2) == new PointClass(1, 2));
Console.WriteLine(new PointRecord(1, 2) == new PointRecord(1, 2));

struct PointStruct(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
}

class PointClass(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
}

record PointRecord(int X, int Y);