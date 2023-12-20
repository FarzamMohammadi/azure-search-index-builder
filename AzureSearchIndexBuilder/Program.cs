using Microsoft.Extensions.Configuration;
using AzureSearchIndexBuilder.Models;
using AzureSearchIndexBuilder;

// Project configuration
var appSettings = GetAppSettings();

// Variables
var storageAccountName = appSettings.AzureStorageAccount.Name;
var storageAccountKey = appSettings.AzureStorageAccount.Key;

var searchServiceEndpoint = appSettings.AzureSearchService.Url;
var searchServiceAdminApiKey = appSettings.AzureSearchService.AdminApiKey;

// Create Books table in Azure Storage
const string tableName = "Books";

var tableServiceManager = new TableServiceManager(storageAccountName, storageAccountKey);

tableServiceManager.CreateTable(tableName);

// Insert data from JSON file into Books table
tableServiceManager.InsertRecordsIntoTable(tableName);

// Builds & prepare the Index, Indexer, and Data Source Connection
var searchServiceManager = new SearchServiceManager
(
    searchServiceEndpoint,
    searchServiceAdminApiKey
);

// Create Search Index infrastructure
var searchIndex = searchServiceManager.CreateIndex();

// Create Data Source Connection for Indexer
var searchIndexerClient = searchServiceManager.CreateIndexerDataSource(storageAccountName, storageAccountKey, tableName, out var dataSourceConnection);

// Create Search Indexer to populate the Index (grabs data from the Data Source Connection and inserts into the Index)
var searchIndexer = SearchServiceManager.CreateIndexer(dataSourceConnection, searchIndex, searchIndexerClient);

// Run Indexer (where data is actually pulled from the Data Source Connection and inserted into the Index)
SearchServiceManager.PopulateIndex(searchIndexerClient, searchIndexer);

// Query data for the fields we setup in the index
var searchIndexDataRetriever = new SearchIndexDataRetriever
(
    searchServiceEndpoint,
    searchServiceAdminApiKey,
    searchIndex
);

var queryOneResults = searchIndexDataRetriever.SearchBooksForGeorgeOrwell();
searchIndexDataRetriever.PrintSearchResults(queryOneResults);

var queryTwoResults = searchIndexDataRetriever.FilterBooksCheaperThanTwentyFiveDollarsInDescendingOrder();
searchIndexDataRetriever.PrintSearchResults(queryTwoResults);

var queryThreeResults = searchIndexDataRetriever.SortBooksByPublishedYearInAscendingOrderAndGrabTopFive();
searchIndexDataRetriever.PrintSearchResults(queryThreeResults);

var queryFourResults = searchIndexDataRetriever.SearchBooksForTitleMatchingMockingBird();
searchIndexDataRetriever.PrintSearchResults(queryFourResults);

return;


AppSettings GetAppSettings()
{
    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    var configuration = builder.Build();

    var appSetting = new AppSettings();
    configuration.Bind(appSetting);

    return appSetting;
}