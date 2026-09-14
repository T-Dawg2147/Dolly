namespace Dolly.Application.Models;

public sealed class SearchNavigationTarget
{
    public string SupplierCode { get; set; } = "";
    public string? ProductCodeOrName { get; set; }
    public string? GroupFilterParentId { get; set; }
}