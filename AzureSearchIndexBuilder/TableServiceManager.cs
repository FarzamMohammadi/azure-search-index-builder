using Azure.Data.Tables;
using AzureSearchIndexBuilder.Models;
using Newtonsoft.Json;

namespace AzureSearchIndexBuilder;

public class TableServiceManager
(
    string storageAccountName,
    string storageAccountKey
)
{
    private readonly TableServiceClient _tableServiceClient = new
    (
        new Uri($"https://{storageAccountName}.table.core.windows.net/"),
        new TableSharedKeyCredential
        (
            storageAccountName,
            storageAccountKey
        )
    );

    public void CreateTable(string tableName)
    {
        _tableServiceClient.CreateTableIfNotExists(tableName);
    }

    public void InsertRecordsIntoTable(string tableName)
    {
        var tableClient = _tableServiceClient.GetTableClient(tableName);

        var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "books.json");
        var jsonFileContent = File.ReadAllText(jsonFilePath);

        var books =
            JsonConvert.DeserializeObject<List<Book>>(jsonFileContent)
            ??
            throw new Exception("Unable to deserialize books. Check JSON file and make sure it was included in the build.");

        foreach (var book in books)
        {
            book.PartitionKey = book.Genre;
            book.RowKey = book.Id;

            tableClient.AddEntity(book);
        }
    }
}