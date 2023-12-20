using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using AzureSearchIndexBuilder.Models;

namespace AzureSearchIndexBuilder;

public class SearchIndexDataRetriever
(
    string searchServiceEndpoint,
    string searchServiceAdminApiKey,
    SearchIndex searchIndex
)
{
    private readonly SearchClient _searchClient = new
    (
        new Uri(searchServiceEndpoint),
        searchIndex.Name,
        new AzureKeyCredential(searchServiceAdminApiKey)
    );

    public SearchResults<Book> SearchBooksForGeorgeOrwell()
    {
        Console.WriteLine("Query 1: Search for 'George Orwell':\n");

        var options = new SearchOptions();

        GrabAllBookFieldsFromTheIndex(options);

        var results = _searchClient.Search<Book>("George Orwell", options);

        return results;
    }

    public SearchResults<Book> FilterBooksCheaperThanTwentyFiveDollarsInDescendingOrder()
    {
        Console.Write("Query 2: Apply a filter to find books cheaper than $25, order by Price in descending order:\n");

        var options = new SearchOptions
        {
            Filter = "Price lt 25",
            OrderBy = { "Price desc" }
        };

        GrabAllBookFieldsFromTheIndex(options);

        var results = _searchClient.Search<Book>("*", options);

        return results;
    }

    public SearchResults<Book> SortBooksByPublishedYearInAscendingOrderAndGrabTopFive()
    {
        Console.Write("Query 3: Search all the books, order by published year in ascending order, take the top 5 results:\n");

        var options = new SearchOptions
        {
            Size = 5,
            OrderBy = { "PublishedYear asc" }
        };

        GrabAllBookFieldsFromTheIndex(options);

        var results = _searchClient.Search<Book>("*", options);

        return results;
    }

    public SearchResults<Book> SearchBooksForTitleMatchingMockingBird()
    {
        Console.WriteLine("Query 4: Search the Title field for the term 'Mockingbird':\n");

        var options = new SearchOptions();
        options.SearchFields.Add("Title");

        GrabAllBookFieldsFromTheIndex(options);

        var results = _searchClient.Search<Book>("Mockingbird", options);

        return results;
    }

    private static void GrabAllBookFieldsFromTheIndex(SearchOptions options)
    {
        options.Select.Add("Id");
        options.Select.Add("Title");
        options.Select.Add("Author");
        options.Select.Add("Genre");
        options.Select.Add("PublishedYear");
        options.Select.Add("Price");
    }
}