using Azure.Search.Documents.Models;
using AzureSearchIndexBuilder.Models;

namespace AzureSearchIndexBuilder;

public static class SearchResultsExtensions
{
    public static void PrintValue(this SearchResults<Book> searchResults)
    {
        foreach (var result in searchResults.GetResults())
        {
            Console.WriteLine(result.Document.ToString());
        }

        Console.WriteLine();
    }
}