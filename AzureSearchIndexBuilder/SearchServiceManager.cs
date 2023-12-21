using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using AzureSearchIndexBuilder.Models;

namespace AzureSearchIndexBuilder;

public class SearchServiceManager
(
    string searchServiceEndpoint,
    string searchServiceAdminApiKey
)
{
    private readonly SearchServiceAuthentication _searchServiceAuthentication = new()
    {
        Uri = new Uri(searchServiceEndpoint),
        Credentials = new AzureKeyCredential(searchServiceAdminApiKey)
    };

    public SearchIndex CreateIndex()
    {
        var searchIndexClient = new SearchIndexClient(_searchServiceAuthentication.Uri, _searchServiceAuthentication.Credentials);
        
        // Define & build the index fields
        var fieldBuilder = new FieldBuilder();
        var searchFields = fieldBuilder.Build(typeof(Book));
        
        // Set the index name and fields
        var searchIndex = new SearchIndex("my-index", searchFields);

        searchIndexClient.CreateOrUpdateIndex(searchIndex);

        return searchIndex;
    }

    public SearchIndexerClient CreateIndexerDataSource
    (
        string storageAccountName,
        string storageAccountKey,
        string tableName,
        out SearchIndexerDataSourceConnection searchIndexerDataSourceConnection
    )
    {
        var searchIndexerClient = new SearchIndexerClient(_searchServiceAuthentication.Uri, _searchServiceAuthentication.Credentials);

        searchIndexerDataSourceConnection = new SearchIndexerDataSourceConnection
        (
            "my-index-data-source",
            SearchIndexerDataSourceType.AzureTable,
            $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey};EndpointSuffix=core.windows.net",
            new SearchIndexerDataContainer(tableName)
        );

        searchIndexerClient.CreateOrUpdateDataSourceConnection(searchIndexerDataSourceConnection);

        return searchIndexerClient;
    }

    public static SearchIndexer CreateIndexer(SearchIndexerDataSourceConnection dataSourceConnection, SearchIndex searchIndex, SearchIndexerClient searchIndexerClient)
    {
        var searchIndexer = new SearchIndexer("my-index-indexer", dataSourceConnection.Name, searchIndex.Name);

        searchIndexerClient.CreateOrUpdateIndexer(searchIndexer);

        return searchIndexer;
    }

    public static void PopulateIndex(SearchIndexerClient searchIndexerClient, SearchIndexer searchIndexer)
    {
        searchIndexerClient.ResetIndexer(searchIndexer.Name);
        
        searchIndexerClient.RunIndexer(searchIndexer.Name);
    }
}