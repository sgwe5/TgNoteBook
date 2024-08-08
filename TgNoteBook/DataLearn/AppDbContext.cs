using Microsoft.EntityFrameworkCore;
using TgNoteBook.DataLearn.Models;

namespace TgNoteBook.DataLearn;

public class AppDbContext : DbContext
{
    public DbSet<NoteBookEntity> NoteBooks { get; set; }
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Note;User Id=postgres;Password=123");
    }
}
