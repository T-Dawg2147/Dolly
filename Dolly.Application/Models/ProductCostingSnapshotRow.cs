namespace Dolly.Application.Models;

public sealed class ProductCostingSnapshotRow
{
    public decimal? Carriage { get; set; }
    public decimal? TotalCost { get; set; }
    public decimal? StandardCost { get; set; }
    public string? DespatchMethod { get; set; }
    public decimal? DespatchCost { get; set; }
}