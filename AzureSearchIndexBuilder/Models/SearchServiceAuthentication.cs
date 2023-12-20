using Azure;

namespace AzureSearchIndexBuilder.Models;

public class SearchServiceAuthentication
{
    public Uri Uri { get; set; } = null!;
    public AzureKeyCredential Credentials { get; set; } = null!;
}