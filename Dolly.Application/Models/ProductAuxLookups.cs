namespace Dolly.Application.Models;

public sealed class ProductAuxLookups
{
    public string? DespatchMethod { get; set; }
    public decimal? DespatchCost { get; set; }
    public decimal? SmallOrderValue { get; set; }
    public decimal? SmallOrderCharge { get; set; }
    public string? LocationPath { get; set; }
    public string? UkPromoPriceText { get; set; }
    public string? RoiPromoPriceText { get; set; }
}