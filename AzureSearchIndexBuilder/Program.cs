using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Configuration;
using AzureSearchIndexBuilder.Models;
using AzureSearchIndexBuilder;

var (storageAccountName, storageAccountKey, searchServiceEndpoint, searchServiceAdminApiKey) = GetProjectVariables();

var tableName = CreateAndPopulateBooksTable(storageAccountName, storageAccountKey);

var searchIndex = CreateAndPopulateBooksSearchIndex(searchServiceEndpoint, searchServiceAdminApiKey, storageAccountName, storageAccountKey, tableName);

// Wait for the Indexer to finish before querying data
Thread.Sleep(30000);

RunQueriesOnSearchIndexAndPrintResults(searchServiceEndpoint, searchServiceAdminApiKey, searchIndex);

return;

ValueTuple<string, string, string, string> GetProjectVariables()
{
    // Project configuration
    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    var configuration = builder.Build();

    var appSettings = new AppSettings();
    configuration.Bind(appSettings);

    // Project variables
    var azureStorageAccountName = appSettings.AzureStorageAccount.Name;
    var azureStorageAccountKey = appSettings.AzureStorageAccount.Key;
    var azureSearchServiceEndpoint = appSettings.AzureSearchService.Url;
    var azureSearchServiceAdminApiKey = appSettings.AzureSearchService.AdminApiKey;

    return (azureStorageAccountName, azureStorageAccountKey, azureSearchServiceEndpoint, azureSearchServiceAdminApiKey);
}

string CreateAndPopulateBooksTable(string azureStorageAccountName, string azureStorageAccountKey)
{
    // Create Books table in Azure Storage
    const string books = "Books";

    var tableServiceManager = new TableServiceManager(azureStorageAccountName, azureStorageAccountKey);

    tableServiceManager.CreateTable(books);

    // Insert data from JSON file into Books table
    tableServiceManager.InsertRecordsIntoTable(books);

    return books;
}

SearchIndex CreateAndPopulateBooksSearchIndex(string azureSearchServiceEndpoint, string azureSearchServiceAdminApiKey, string azureStorageAccountName, string azureStorageAccountKey, string storageTableName)
{
    // Builds & prepare the Index, Indexer, and Data Source Connection
    var searchServiceManager = new SearchServiceManager
    (
        azureSearchServiceEndpoint,
        azureSearchServiceAdminApiKey
    );

    // Create Search Index infrastructure
    var azureSearchIndex = searchServiceManager.CreateIndex();

    // Create Data Source Connection for Indexer
    var searchIndexerClient = searchServiceManager.CreateIndexerDataSource(azureStorageAccountName, azureStorageAccountKey, storageTableName, out var dataSourceConnection);

    // Create Search Indexer to populate the Index (grabs data from the Data Source Connection and inserts into the Index)
    var searchIndexer = SearchServiceManager.CreateIndexer(dataSourceConnection, azureSearchIndex, searchIndexerClient);

    // Run Indexer (where data is actually pulled from the Data Source Connection and inserted into the Index)
    SearchServiceManager.PopulateIndex(searchIndexerClient, searchIndexer);

    return azureSearchIndex;
}

void RunQueriesOnSearchIndexAndPrintResults(string azureSearchServiceEndpoint, string azureSearchServiceAdminApiKey, SearchIndex azureSearchIndex)
{
    // Query data for the fields we setup in the index
    var searchIndexDataRetriever = new SearchIndexDataRetriever
    (
        azureSearchServiceEndpoint,
        azureSearchServiceAdminApiKey,
        azureSearchIndex
    );

    var queryOneResults = searchIndexDataRetriever.SearchBooksForGeorgeOrwell();
    queryOneResults.PrintValue();

    var queryTwoResults = searchIndexDataRetriever.FilterBooksCheaperThanTwentyFiveDollarsInDescendingOrder();
    queryTwoResults.PrintValue();

    var queryThreeResults = searchIndexDataRetriever.SortBooksByPublishedYearInAscendingOrderAndGrabTopFive();
    queryThreeResults.PrintValue();

    var queryFourResults = searchIndexDataRetriever.SearchBooksForTitleMatchingMockingBird();
    queryFourResults.PrintValue();
}