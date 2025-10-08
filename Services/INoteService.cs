using PersonalNotesApi.DTOs;

namespace PersonalNotesApi.Services;

public interface INoteService
{
    Task<IEnumerable<NoteReadDto>> GetAllAsync(string? search = null, string? tag = null);
    Task<NoteReadDto?> GetByIdAsync(int id);
    Task<NoteReadDto> CreateAsync(NoteCreateDto dto);
    Task<NoteReadDto?> UpdateAsync(int id, NoteUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
