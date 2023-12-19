// Azure Storage Table creation
// Do this in portal

using Azure;
using Microsoft.Extensions.Configuration;
using AzureSearchIndexBuilder.Models;
using Azure.Data.Tables;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Newtonsoft.Json;

// Project configuration
var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Build();

var appSettings = new AppSettings();
configuration.Bind(appSettings);

// Table creation and data insertion
var storageAccountName = appSettings.AzureStorageAccount.Name;
var storageAccountKey = appSettings.AzureStorageAccount.Key;

var tableServiceClient = new TableServiceClient
(
    new Uri($"https://{storageAccountName}.table.core.windows.net/"),
    new TableSharedKeyCredential
    (
        storageAccountName,
        storageAccountKey
    )
);

const string tableName = "Books";
tableServiceClient.CreateTableIfNotExists(tableName);

var tableClient = tableServiceClient.GetTableClient(tableName);

var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "books.json");
var jsonFileContent = File.ReadAllText(jsonFilePath);
var books = JsonConvert.DeserializeObject<List<Book>>(jsonFileContent) ?? throw new Exception("Unable to deserialize books. Check JSON file and make sure it was included in the build.");

foreach (var book in books)
{
    book.PartitionKey = book.Genre;
    book.RowKey = book.Id;

    tableClient.AddEntity(book);
}

// Create Entity (with filterable and sortable and whatever else fields)

// Create Search Service
// Do this in portal

// Prepare Search Service client
// Create Index
var searchServiceEndpoint = appSettings.AzureSearchService.Url;
var searchServiceAdminApiKey = appSettings.AzureSearchService.AdminApiKey;

var searchIndexClient =
    new SearchIndexClient(new Uri(searchServiceEndpoint), new AzureKeyCredential(searchServiceAdminApiKey));

var fieldBuilder = new FieldBuilder();
var searchFields = fieldBuilder.Build(typeof(Book));

var searchIndex = new SearchIndex("my-index", searchFields);
searchIndexClient.CreateOrUpdateIndex(searchIndex);

// Create Data Source Connection
var searchIndexerClient = new SearchIndexerClient(new Uri(searchServiceEndpoint), new AzureKeyCredential(searchServiceAdminApiKey));

var dataSourceConnection = new SearchIndexerDataSourceConnection
(
    "my-index-data-source",
    SearchIndexerDataSourceType.AzureTable,
    $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey};EndpointSuffix=core.windows.net",
    new SearchIndexerDataContainer(tableName)
);

searchIndexerClient.CreateOrUpdateDataSourceConnection(dataSourceConnection);

// Create Indexer
var searchIndexer = new SearchIndexer("my-index-indexer", dataSourceConnection.Name, searchIndex.Name);
searchIndexerClient.CreateOrUpdateIndexer(searchIndexer);

// Run Indexer
searchIndexerClient.ResetIndexer(searchIndexer.Name);
searchIndexerClient.RunIndexer(searchIndexer.Name);

// Query data for the fields we setup in the index
