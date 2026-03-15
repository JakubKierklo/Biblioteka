DatabaseInitializer.Initialize();


using var db = new LibraryContext();

db.Database.EnsureCreated();

if (!db.Books.Any())
{
    var fakeBooks = Enumerable.Range(1, 50)
        .Select(i => new Book { Title = $"Książka {i}", Author = "Autor", IsAvailable = i % 2 == 0 });

    db.Books.AddRange(fakeBooks);
    db.SaveChanges();
}

Console.WriteLine("w");