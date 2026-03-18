using Biblioteka.Services;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static async Task Main(string[] args)
    {
        DatabaseInitializer.Initialize();

        using var db = new LibraryContext();

        db.Database.EnsureCreated();
        await ZasilDane(db);

        // TEST 1: Symulacja awarii i rollback
        Console.WriteLine($"\n=== TEST: Symulacja awarii z rollback ===");
        using var crashDb = new LibraryContext();
        var crashService = new BorrowService(crashDb);

        Console.WriteLine("[TEST] Próba wypożyczenia z symulowaną awarią...");
        var crashResult = await crashService.BorrowWithCrashAsync(2, 99);
        Console.WriteLine($"[TEST] Wynik: {(crashResult ? "SUKCES" : "PORAŻKA")}");
 
        using var verifyDb = new LibraryContext();
        var bookAfterCrash = await verifyDb.Books.FindAsync(2);
        Console.WriteLine($"[TEST] Książka #2 IsAvailable po awarii: {bookAfterCrash?.IsAvailable} (powinno być true)");  
        
        Console.WriteLine("\n=== TEST 2");
        var cycleTasks = new List<Task<(int userId, int bookId, bool result)>>();
        for (int i = 0; i < 50; i++)
        {
            int userId = 300 + i;
            int bookId = 5 + (i % 10);
            cycleTasks.Add(Task.Run(async () =>
            {
                using var taskDb = new LibraryContext();
                var svc = new BorrowService(taskDb);
                bool result = await svc.BorrowBookAsync(bookId, userId);
                return (userId, bookId, result);
            }));
        }

        var cycleResults = await Task.WhenAll(cycleTasks);
        var succeeded = cycleResults.Where(r => r.result).ToList();
        Console.WriteLine($"Sukcesy: {succeeded.Count} / Porażki: {cycleResults.Count(r => !r.result)}");
     }

    private static async Task ZasilDane(LibraryContext db)
    {
        if (!db.Books.Any())
        {
            var fakeBooks = Enumerable.Range(1, 50)
                .Select(i => new Book { Title = $"Książka {i}", Author = "Autor", IsAvailable = false });

            db.Books.AddRange(fakeBooks);
            await db.SaveChangesAsync();
        }
        else
        {
            foreach (var item in await db.Books.ToListAsync())
            {
                item.IsAvailable = true;
            }

            await db.SaveChangesAsync();
        }
    }
}