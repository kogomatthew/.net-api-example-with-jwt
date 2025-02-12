using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Authorize]
[ApiController]
[Route("[controller]")]
public class TodoController: ControllerBase
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
    {
        return await _context.Todos.ToListAsync();
    }


    [HttpPost]
    public async Task<ActionResult<Todo>> PostTodo(Todo todo)
    {
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTodos), new { id = todo.Id}, todo);
    }
}