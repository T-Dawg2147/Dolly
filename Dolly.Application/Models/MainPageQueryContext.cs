namespace Dolly.Application.Models;

public sealed class MainPageQueryContext
{
    public string SupplierCode { get; set; } = "";
    public int ProductStatus { get; set; } = 1;
    public bool ReportingSupplier { get; set; }
    public bool ContainsSupplier { get; set; } 
}