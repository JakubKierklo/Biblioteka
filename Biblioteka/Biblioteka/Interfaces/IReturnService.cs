namespace Biblioteka.Interfaces;

internal interface IReturnService
{
    Task<bool> ReturnBookAsync(int bookId, int userId);
}
