using Microsoft.Data.Sqlite;

public static class DatabaseInitializer
{
    private const string ConnectionString = "Data Source=biblioteka.db";

    public static void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var createBooksTable = @"
            CREATE TABLE IF NOT EXISTS Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL,
                IsAvailable BOOLEAN NOT NULL
            );";

        var createBorrowingsTable = @"
            CREATE TABLE IF NOT EXISTS Borrowings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BookId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                BorrowedAt DATETIME NOT NULL,
                ReturnedAt DATETIME NULL,
                FOREIGN KEY (BookId) REFERENCES Books(Id)
            );";

        using var command = connection.CreateCommand();

        command.CommandText = createBooksTable;
        command.ExecuteNonQuery();

        command.CommandText = createBorrowingsTable;
        command.ExecuteNonQuery();

        Console.WriteLine("Baza danych została zainicjalizowana pomyślnie.");
    }
}