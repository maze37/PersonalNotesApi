using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PersonalNotesApi.Data;
using PersonalNotesApi.DTOs;
using PersonalNotesApi.Models;

namespace PersonalNotesApi.Services;

public class NoteService : INoteService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContext;

    public NoteService(AppDbContext db, IHttpContextAccessor httpContext)
    {
        _db = db;
        _httpContext = httpContext;
    }

    private int GetUserId()
    {
        var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            throw new UnauthorizedAccessException("Пользователь не авторизован");

        return int.Parse(userId);
    }

    public async Task<IEnumerable<NoteReadDto>> GetAllAsync(string? search = null, string? tag = null)
    {
        var userId = GetUserId();

        var query = _db.Notes
            .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
            .Where(n => n.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(n =>
                EF.Functions.ILike(n.Title, $"%{search}%") ||
                EF.Functions.ILike(n.Content, $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            query = query.Where(n => n.NoteTags.Any(nt => nt.Tag.Name.ToLower() == tag.ToLower()));
        }

        var notes = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();

        return notes.Select(n => new NoteReadDto
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            CreatedAt = n.CreatedAt,
            Tags = n.NoteTags.Select(nt => nt.Tag.Name).ToList()
        });
    }

    public async Task<NoteReadDto?> GetByIdAsync(int id)
    {
        var userId = GetUserId();

        var note = await _db.Notes
            .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
            return null;

        return new NoteReadDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            Tags = note.NoteTags.Select(nt => nt.Tag.Name).ToList()
        };
    }

    public async Task<NoteReadDto> CreateAsync(NoteCreateDto dto)
    {
        var userId = GetUserId();

        var note = new Note
        {
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        if (dto.Tags != null && dto.Tags.Any())
        {
            var tags = await GetOrCreateTagsAsync(dto.Tags);
            note.NoteTags = tags.Select(t => new NoteTag { TagId = t.Id }).ToList();
        }

        _db.Notes.Add(note);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(note.Id) ?? throw new Exception("Не удалось создать заметку");
    }

    public async Task<NoteReadDto?> UpdateAsync(int id, NoteUpdateDto dto)
    {
        var userId = GetUserId();

        var note = await _db.Notes
            .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
            return null;

        note.Title = dto.Title;
        note.Content = dto.Content;

        if (dto.Tags != null)
        {
            note.NoteTags.Clear();
            var tags = await GetOrCreateTagsAsync(dto.Tags);
            note.NoteTags = tags.Select(t => new NoteTag { NoteId = note.Id, TagId = t.Id }).ToList();
        }

        await _db.SaveChangesAsync();

        return await GetByIdAsync(note.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var userId = GetUserId();

        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note == null)
            return false;

        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task<List<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames)
    {
        var normalized = tagNames.Select(t => t.Trim().ToLower()).ToList();

        var existing = await _db.Tags
            .Where(t => normalized.Contains(t.Name.ToLower()))
            .ToListAsync();

        var newTags = normalized
            .Except(existing.Select(t => t.Name.ToLower()))
            .Select(t => new Tag { Name = t })
            .ToList();

        if (newTags.Any())
        {
            _db.Tags.AddRange(newTags);
            await _db.SaveChangesAsync();
        }

        return existing.Concat(newTags).ToList();
    }
}
