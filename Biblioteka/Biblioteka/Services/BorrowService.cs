using Biblioteka.Interfaces;

namespace Biblioteka.Services;

internal class BorrowService : IBorrowService
{
    private readonly LibraryContext _context;

    public BorrowService(LibraryContext context)
    {
        _context = context;
    }

    public async Task<bool> BorrowBookAsync(int bookId, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;
            
            book.IsAvailable = false;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();

            Console.WriteLine($"[LOG] Rollback wykonany! Powód: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> BorrowWithCrashAsync(int bookId, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            book.IsAvailable = false;
            Console.WriteLine($"[LOG] Zmieniono status książki {bookId} na false.");

            Console.WriteLine("[LOG] !!! SYMULACJA AWARII !!!");
            throw new InvalidOperationException("Krytyczny błąd systemu podczas zapisu!");

            // Ten kod nigdy się nie wykona:
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();

            Console.WriteLine($"[LOG] Rollback wykonany! Powód: {ex.Message}");
            return false;
        }
    }
}
