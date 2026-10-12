using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Services;

namespace TodoApi.Tests;

public sealed class TodoServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly TodoService _sut; 
    public TodoServiceTests()
    {
        var connectionString = $"Host=localhost;Port=5432;Database=todo_test_{Guid.NewGuid():N};Username=postgres";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
        _sut = new TodoService(_db);
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }

    [Fact]
    public async Task CreateAsync_TrimsTitleAndSetsCreatedAt()
    {
        var item = await _sut.CreateAsync("  Buy milk  ");

        Assert.Equal("Buy milk", item.Title);
        Assert.False(item.IsCompleted);
        Assert.True(item.Id > 0);
        Assert.True(item.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_BlankTitle_Throws(string title)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(title));
    }

    [Fact]
    public async Task UpdateAsync_MarkingComplete_SetsCompletedAt()
    {
        var created = await _sut.CreateAsync("Write tests");

        var updated = await _sut.UpdateAsync(created.Id, "Write tests", true);

        Assert.NotNull(updated);
        Assert.True(updated!.IsCompleted);
        Assert.NotNull(updated.CompletedAt);
    }

    [Fact]
    public async Task UpdateAsync_MarkingIncomplete_ClearsCompletedAt()
    {
        var created = await _sut.CreateAsync("Write tests");
        await _sut.UpdateAsync(created.Id, "Write tests", true);

        var updated = await _sut.UpdateAsync(created.Id, "Write tests", false);

        Assert.NotNull(updated);
        Assert.False(updated!.IsCompleted);
        Assert.Null(updated.CompletedAt);
    }

    [Fact]
    public async Task UpdateAsync_UnknownId_ReturnsNull()
    {
        var result = await _sut.UpdateAsync(9999, "Nope", false);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingItem_RemovesItAndReturnsTrue()
    {
        var created = await _sut.CreateAsync("Temporary");

        var deleted = await _sut.DeleteAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(await _sut.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task DeleteAsync_UnknownId_ReturnsFalse()
    {
        Assert.False(await _sut.DeleteAsync(9999));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsItemsInCreationOrder()
    {
        await _sut.CreateAsync("First");
        await _sut.CreateAsync("Second");

        var all = await _sut.GetAllAsync();

        Assert.Equal(new[] { "First", "Second" }, all.Select(t => t.Title));
    }
}