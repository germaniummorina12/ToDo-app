using System.Runtime.InteropServices;

Console.WriteLine(RuntimeInformation.FrameworkDescription);
Console.WriteLine(Environment.Version);
Console.WriteLine(RuntimeInformation.OSDescription);
Console.WriteLine(RuntimeInformation.OSArchitecture);
Console.WriteLine(RuntimeInformation.ProcessArchitecture);
Console.WriteLine(typeof(object).Assembly.Location);
Console.WriteLine(AppContext.BaseDirectory);