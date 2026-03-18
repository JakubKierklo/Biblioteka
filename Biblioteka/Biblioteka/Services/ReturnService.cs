using Biblioteka.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Services;

internal class ReturnService : IReturnService
{
    private readonly LibraryContext _context;

    public ReturnService(LibraryContext context)
    {
        _context = context;
    }

    public async Task<bool> ReturnBookAsync(int bookId, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.IsAvailable) return false;

            var borrowing = await _context.Borrowings
                .Where(b => b.BookId == bookId && b.UserId == userId && b.ReturnedAt == null)
                .FirstOrDefaultAsync();

            if (borrowing == null) return false;

            borrowing.ReturnedAt = DateTime.UtcNow;
            book.IsAvailable = true;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            Console.WriteLine($"[LOG] Rollback zwrotu! Powód: {ex.Message}");
            return false;
        }
    }
}
