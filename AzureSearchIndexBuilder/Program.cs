// Azure Storage Table creation
// Do this in portal

using Microsoft.Extensions.Configuration;
using AzureSearchIndexBuilder.Models;
using Azure.Data.Tables;
using Newtonsoft.Json;

// Project configuration
var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Build();

var appSettings = new AppSettings();
configuration.Bind(appSettings);

// Table creation and data insertion
var storageAccountName = appSettings.AzureStorageAccount.Name;
var storageAccountKey = appSettings.AzureStorageAccount.Key;

var serviceClient = new TableServiceClient
(
    new Uri($"https://{storageAccountName}.table.core.windows.net/"),
    new TableSharedKeyCredential
    (
        storageAccountName,
        storageAccountKey
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