using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace AzureSearchIndexBuilder.Models;

public class Book : ITableEntity
{
    [JsonIgnore] // Ignored by Index & Indexer.
    public string PartitionKey { get; set; } = null!;

    [JsonIgnore] // Ignored by Index & Indexer.
    public string RowKey { get; set; } = null!;

    [JsonIgnore] // Ignored by Index & Indexer.
    public DateTimeOffset? Timestamp { get; set; }

    [JsonIgnore] // Ignored by Index & Indexer.
    public ETag ETag { get; set; }

    [SimpleField(IsKey = true)]
    public string Id { get; set; } = null!;

    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
    public string Title { get; set; } = null!;

    [SearchableField(IsFilterable = true)]
    public string Author { get; set; } = null!;

    [SimpleField(IsFilterable = true)]
    public string Genre { get; set; } = null!;

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public int PublishedYear { get; set; }

    [SimpleField(IsSortable = true)]
    public double Price { get; set; }
}

// Sortable Fields: PublishedYear and Price. These fields are numerical and inherently sortable.
// Filterable Fields: Author, Genre, and PublishedYear. It's a categorical field suitable for filtering.
// Searchable Fields: Title and Author. These are textual fields, perfect for full-text search.