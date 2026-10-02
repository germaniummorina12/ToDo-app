var o = new object();

Console.WriteLine($"new object      : gen {GC.GetGeneration(o)}");
GC.Collect();

Console.WriteLine($"after 1 collect: gen {GC.GetGeneration(o)}");
GC.Collect();

Console.WriteLine($"after 2 collect: gen {GC.GetGeneration(o)}");

var big = new byte[100_000];
Console.WriteLine($"100 KB array    : gen {GC.GetGeneration(big)}");