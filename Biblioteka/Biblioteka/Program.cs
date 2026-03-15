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

        var tasks = new List<Task<bool>>();
        var borrowService = new BorrowService(db);


        for (int i = 0; i < 5; i++)
        {
            int userId = i; 
            
            tasks.Add(Task.Run(() => borrowService.BorrowBookAsync(1, userId)));
        }
        
        // Czekamy, aż wszystkie się zakończą
        bool[] results = await Task.WhenAll(tasks);

        Console.WriteLine($"Sukcesy: {results.Count(r => r == true)}");
        Console.WriteLine($"Porażki: {results.Count(r => r == false)}");
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