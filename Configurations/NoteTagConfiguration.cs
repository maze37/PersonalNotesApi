using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalNotesApi.Models;

namespace PersonalNotesApi.Configurations;

public class NoteTagConfiguration : IEntityTypeConfiguration<NoteTag>
{
    public void Configure(EntityTypeBuilder<NoteTag> builder)
    {
        builder.HasKey(nt => new { nt.NoteId, nt.TagId });
    }
}
