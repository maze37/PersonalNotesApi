namespace PersonalNotesApi.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();
}
