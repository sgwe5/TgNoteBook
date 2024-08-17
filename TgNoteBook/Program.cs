using TgNoteBook.DataLearn;
using TgNoteBook.DataLearn.Repos;

namespace TgNoteBook;

class Program
{
    static void Main(string[] args)
    {
        using var dbContext = new AppDbContext();
        
        NoteBookRepos noteBookRepos = new NoteBookRepos(dbContext);
        
        Bot bot = new Bot(noteBookRepos);
        bot.Start();
        Console.ReadKey();  
    }
}