using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddTransient<TransientService>();
services.AddScoped<ScopedService>();
services.AddSingleton<SingletonService>();
services.AddSingleton<Captive>();

using var provider = services.BuildServiceProvider(validateScopes: true);

Console.WriteLine("--- root provider");
Console.WriteLine(provider.GetRequiredService<Captive>().Id);
Console.WriteLine($"singleton: {provider.GetRequiredService<SingletonService>().Id}");
Console.WriteLine($"singleton: {provider.GetRequiredService<SingletonService>().Id}");

for (var request = 1; request <= 2; request++)
{
    using var scope = provider.CreateScope();
    Console.WriteLine($"--- scope {request}");
    Console.WriteLine($"scoped: {scope.ServiceProvider.GetRequiredService<ScopedService>().Id}");
    Console.WriteLine($"scoped: {scope.ServiceProvider.GetRequiredService<ScopedService>().Id}");
    Console.WriteLine($"transient: {scope.ServiceProvider.GetRequiredService<TransientService>().Id}");
    Console.WriteLine($"transient: {scope.ServiceProvider.GetRequiredService<TransientService>().Id}");
}

class TransientService { public Guid Id { get; } = Guid.NewGuid(); }
class ScopedService { public Guid Id { get; } = Guid.NewGuid(); }
class SingletonService { public Guid Id { get; } = Guid.NewGuid(); }
class Captive(ScopedService scoped) { public Guid Id => scoped.Id; }
