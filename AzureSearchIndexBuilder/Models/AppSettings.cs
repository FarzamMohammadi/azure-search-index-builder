namespace AzureSearchIndexBuilder.Models;

public class AppSettings
{
    public AzureStorageAccount AzureStorageAccount { get; set; }
    public AzureSearchService AzureSearchService { get; set; }
}

public class AzureStorageAccount
{
    public string Name { get; set; }
    public string Key { get; set; }
}

public class AzureSearchService
{
    public string Url { get; set; }
    public string AdminApiKey { get; set; }
}