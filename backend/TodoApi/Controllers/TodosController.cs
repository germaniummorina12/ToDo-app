using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController(ITodoService todos) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoDto>>> GetAll(CancellationToken ct)
    {
        var items = await todos.GetAllAsync(ct);
        return Ok(items.Select(i => i.ToDto()).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoDto>> GetById(int id, CancellationToken ct)
    {
        var item = await todos.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<TodoDto>> Create(CreateTodoRequest request, CancellationToken ct)
    {
        var created = await todos.CreateAsync(request.Title, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoDto>> Update(int id, UpdateTodoRequest request, CancellationToken ct)
    {
        var updated = await todos.UpdateAsync(id, request.Title, request.IsCompleted, ct);
        return updated is null ? NotFound() : Ok(updated.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await todos.DeleteAsync(id, ct) ? NoContent() : NotFound();
}