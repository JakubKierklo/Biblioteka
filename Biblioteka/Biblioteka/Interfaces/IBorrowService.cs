namespace Biblioteka.Interfaces;

internal interface IBorrowService
{
    Task<bool> BorrowBookAsync(int bookId, int userId);

    Task<bool> BorrowWithCrashAsync(int bookId, int userId);
}
