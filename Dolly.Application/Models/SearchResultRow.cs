namespace Dolly.Application.Models;

public sealed class SearchResultRow
{
    public string? Location { get; set; }
    public string? FoundIn { get; set; }
    public string? Found { get; set; }
    public string? SearchValue { get; set; }
    public string? KeyValue { get; set; }
    public string? ExactFound { get; set; } 
}