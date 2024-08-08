namespace TgNoteBook.DataLearn.Models;

public class NoteBookEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Text{ get; set; }
    public DateTime Created { get; set; }
    public UserEntity User { get; set; }
}
