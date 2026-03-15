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

        using var command = connection.CreateCommand();
        
        command.CommandText = createBooksTable;
        command.ExecuteNonQuery();

        Console.WriteLine("Baza danych została zainicjalizowana pomyślnie.");
    }
}