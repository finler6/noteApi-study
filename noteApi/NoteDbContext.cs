using Microsoft.EntityFrameworkCore;

namespace noteApi;

public class NoteDbContext : DbContext
{
    public DbSet<Note> Notes { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=notes.db");
    }
}