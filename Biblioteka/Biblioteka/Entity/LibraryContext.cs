using Microsoft.EntityFrameworkCore;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

public class Borrowing
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int UserId { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}

public class LibraryContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Borrowing> Borrowings => Set<Borrowing>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=biblioteka.db");
}