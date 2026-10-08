using System.Diagnostics;
using Library.Api.Data;
using Library.Api.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Library")));
builder.Services.Configure<LibraryOptions>(builder.Configuration.GetSection(LibraryOptions.SectionName));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();


app.Use(async (context, next) =>
{
    var started = Stopwatch.GetTimestamp();
    await next(context);
    app.Logger.LogInformation("{Method} {Path} -> {Status} in {Ms:F1} ms",
        context.Request.Method, context.Request.Path, context.Response.StatusCode,
        Stopwatch.GetElapsedTime(started).TotalMilliseconds);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using var scope = app.Services.CreateScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<LibraryDbContext>());
}

app.MapControllers();

app.Run();
