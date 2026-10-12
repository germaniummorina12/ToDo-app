using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todos;

    public TodosController(ITodoService todos)
    {
        _todos = todos;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoItem>>> GetAll()
    {
        return Ok(await _todos.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
        var item = await _todos.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(CreateTodoRequest request)
    {
        var created = await _todos.CreateAsync(request.Title);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoItem>> Update(int id, UpdateTodoRequest request)
    {
        var updated = await _todos.UpdateAsync(id, request.Title, request.IsCompleted);
        if (updated is null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _todos.DeleteAsync(id);
        if(!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
