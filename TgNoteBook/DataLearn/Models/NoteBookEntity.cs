namespace TgNoteBook.DataLearn.Models;

public class NoteBookEntity
{
    public int Id { get; set; }
    public long TgChatId { get; set; }
    public string Title { get; set; }
    public string Text{ get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}
