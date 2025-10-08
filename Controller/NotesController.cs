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

    /// <summary>
    /// Получить все заметки текущего пользователя
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? tag)
    {
        var notes = await _service.GetAllAsync(search, tag);
        return Ok(notes);
    }

    /// <summary>
    /// Получить заметку по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var note = await _service.GetByIdAsync(id);
        if (note == null)
            return NotFound(new { message = "Заметка не найдена" });

        return Ok(note);
    }

    /// <summary>
    /// Создать новую заметку
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NoteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var note = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    /// <summary>
    /// Обновить заметку
    /// </summary>
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

    /// <summary>
    /// Удалить заметку
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(new { message = "Заметка не найдена или уже удалена" });

        return NoContent();
    }
}
