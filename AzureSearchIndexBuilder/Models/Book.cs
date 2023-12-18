using Azure;
using Azure.Data.Tables;

namespace AzureSearchIndexBuilder.Models;

public class Book : ITableEntity
{
    public string PartitionKey { get; set; } = null!;
    public string RowKey { get; set; } = null!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public int PublishedYear { get; set; }
    public double Price { get; set; }
}

// Sortable Fields: PublishedYear and Price. These fields are numerical and inherently sortable.
// Filterable Field: Genre. It's a categorical field suitable for filtering.
// Searchable Fields: Title and Author. These are textual fields, perfect for full-text search.