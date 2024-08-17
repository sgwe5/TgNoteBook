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

    public async Task<List<NoteBookEntity>> GetNotes(long chatId)
    {
        return await _dbContext.NoteBooks
            .Where(c => c.TgChatId == chatId)
            .ToListAsync();
    }

    public async Task<NoteBookEntity> GetNoteTitle(long chatId, string title)
    {
        return await _dbContext.NoteBooks
            .FirstOrDefaultAsync(n => n.TgChatId == chatId && n.Title == title);
    }

    public async Task AddAsync(NoteBookEntity noteBook)
    {
        var note = new NoteBookEntity
        {
            Id = noteBook.Id,
            TgChatId = noteBook.TgChatId,
            Title = noteBook.Title,
            Text = noteBook.Text,
            Created = noteBook.Created
        };
        _dbContext.NoteBooks.AddAsync(note);
        await _dbContext.SaveChangesAsync();
    }

    public void Update(int id)
    {
        var noteBook = _dbContext.NoteBooks.FirstOrDefault(c => c.Id == id)
                       ?? throw new Exception();
        noteBook.Id = noteBook.Id;
        noteBook.TgChatId = noteBook.TgChatId;
        noteBook.Title = noteBook.Title;
        noteBook.Text = noteBook.Text;
        noteBook.Created = noteBook.Created;

        _dbContext.SaveChanges();
    }

    public async Task DeleteTitleAsync(long chatId, string title)
    {
        await _dbContext.NoteBooks
            .Where(c => c.TgChatId == chatId && c.Title == title)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteAllAsync(long chatId)
    {
        await _dbContext.NoteBooks
            .Where(c => c.TgChatId == chatId)
            .ExecuteDeleteAsync();
    }
}