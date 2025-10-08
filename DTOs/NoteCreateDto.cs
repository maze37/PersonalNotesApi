namespace PersonalNotesApi.DTOs;

public class NoteCreateDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public List<string>? Tags { get; set; }
}
