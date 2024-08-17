using Microsoft.EntityFrameworkCore;
using TgNoteBook.DataLearn.Configuration;
using TgNoteBook.DataLearn.Models;

namespace TgNoteBook.DataLearn;

public class AppDbContext : DbContext
{
    public DbSet<NoteBookEntity> NoteBooks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Note;User Id=postgres;Password=123");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NoteBookConfig());
        base.OnModelCreating(modelBuilder);
    }
}
