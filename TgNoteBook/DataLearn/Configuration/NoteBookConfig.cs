using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TgNoteBook.DataLearn.Models;

namespace TgNoteBook.DataLearn.Configuration;

public class NoteBookConfig : IEntityTypeConfiguration<NoteBookEntity>
{
    public void Configure(EntityTypeBuilder<NoteBookEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(c => c.TgChatId)
            .IsRequired();
        builder.Property(c => c.Title)
            .IsRequired();
        builder.Property(c => c.Text)
            .IsRequired();
    }
}
