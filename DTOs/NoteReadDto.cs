namespace PersonalNotesApi.DTOs;

public class NoteReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
}
