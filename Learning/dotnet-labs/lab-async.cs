using System.Diagnostics;

const int Jobs = 200;
Console.WriteLine($"CPU cores: {Environment.ProcessorCount}");

var sw = Stopwatch.StartNew();
await Task.WhenAll(Enumerable.Range(0, Jobs).Select(async _ => await Task.Delay(1000)));
Console.WriteLine($"async: {Jobs} one-second waits finished in {sw.Elapsed.TotalSeconds:F1}s, threads in pool: {ThreadPool.ThreadCount}");


sw.Restart();
await Task.WhenAll(Enumerable.Range(0, Jobs).Select(_ => Task.Run(() => Thread.Sleep(1000))));
Console.WriteLine($"blocked: {Jobs} one-second sleeps finished in {sw.Elapsed.TotalSeconds:F1}s, threads in pool: {ThreadPool.ThreadCount}");