// Azure Storage Table creation
// Do this in portal

// Table creation and data insertion

using Azure.Data.Tables;
using AzureSearchIndexBuilder.Models;
using Newtonsoft.Json;

var serviceClient = new TableServiceClient
(
    new Uri("TABLE_ENDPOINT - E.G: https://{STORAGE_ACCOUNT_NAME}.table.core.windows.net/"),
    new TableSharedKeyCredential
    (
        "STORAGE_ACCOUNT_NAME - Displayed at the very top of the 'Storage Account' overview.",
        "STORAGE_ACCOUNT_KEY - Available in the portal under 'Access Keys'"
    )
);

const string tableName = "Books";
serviceClient.CreateTableIfNotExists(tableName);

var tableClient = serviceClient.GetTableClient(tableName);

var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "books.json");
var jsonFileContent = File.ReadAllText(jsonFilePath);
var books = JsonConvert.DeserializeObject<List<Book>>(jsonFileContent) ?? throw new Exception("Unable to deserialize books. Check JSON file and make sure it was included in the build.");

foreach (var book in books)
{
    book.PartitionKey = book.Genre;
    book.RowKey = book.Id.ToString();

    await tableClient.AddEntityAsync(book);
}

// Create Entity (with filterable and sortable and whatever else fields)

// Create Data Source Connection
// Create Indexer
// Create Index
// Query data for the fields we setup in the index

Console.WriteLine("sadf");