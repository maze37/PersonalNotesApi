using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalNotesApi.DTOs;
using PersonalNotesApi.Services;

namespace PersonalNotesApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _service;

    public NotesController(INoteService service)
    {
        _service = service;
    }

    // Получить все заметки текущего пользователя
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? tag)
    {
        var notes = await _service.GetAllAsync(search, tag);
        return Ok(notes);
    }

    // Получить заметку по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var note = await _service.GetByIdAsync(id);
        if (note == null)
            return NotFound(new { message = "Заметка не найдена" });

        return Ok(note);
    }

    // Создать новую заметку
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NoteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var note = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    // Обновить заметку
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] NoteUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var note = await _service.UpdateAsync(id, dto);
        if (note == null)
            return NotFound(new { message = "Заметка не найдена или недоступна" });

        return Ok(note);
    }

    // Удалить заметку
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(new { message = "Заметка не найдена или уже удалена" });

        return NoContent();
    }
}
