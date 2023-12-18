namespace AzureSearchIndexBuilder.Models;

public class AppSettings
{
    public AzureStorageAccount AzureStorageAccount { get; set; }
}

public class AzureStorageAccount
{
    public string Name { get; set; }
    public string Key { get; set; }
}