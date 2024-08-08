using Microsoft.EntityFrameworkCore;
using TgNoteBook.DataLearn.Models;

namespace TgNoteBook.DataLearn.Repos;

public class NoteBookRepos
{
    private readonly AppDbContext _dbContext;

    public NoteBookRepos(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public List<NoteBookEntity> GetNotes()
    {
        return _dbContext.NoteBooks
        .OrderBy(c => c.Id)
        .ToList();
    }
    public void Add(NoteBookEntity noteBook)
    {
        var note = new NoteBookEntity
        {
            Id = noteBook.Id,
            Name = noteBook.Name,
            Text = noteBook.Text,
            Created = noteBook.Created,
            User = noteBook.User,
        };
        _dbContext.NoteBooks.Add(note);
        _dbContext.SaveChanges();
    }
    public void Update(int id)
    {
        var noteBook = _dbContext.NoteBooks.FirstOrDefault(c => c.Id == id)
            ?? throw new Exception();
        noteBook.Id = noteBook.Id;
        noteBook.Name = noteBook.Name;
        noteBook.Text = noteBook.Text;
        noteBook.Created = noteBook.Created;

        _dbContext.SaveChanges();
    }
    public void Delete(int id)
    {
        _dbContext.NoteBooks
            .Where(c => c.Id == id)
            .ExecuteDelete();
    }
}
