namespace Dolly.Application.Models;

public sealed class SupplierProductSalesRow
{
    public string? Section { get; set; }
    public string? ProductManager { get; set; }       // SI.Product_Manager
    public string? SupplierCode { get; set; }
    public string? SupplierProductCode { get; set; }  // W.Supplier_Product_Code
    public string? Heading { get; set; }
    public int? PageNo { get; set; }
    public string? DesignNo { get; set; }
    public string? ParentId { get; set; }             // W.Parent_ID
    public string? ObjectType { get; set; }
    public string? LongDescription1 { get; set; }
    public string? LongDescription2 { get; set; }

    public int? OrdersTwoYear { get; set; }
    public int? UnitsTwoYear { get; set; }
    public decimal? CostsTwoYear { get; set; }
    public decimal? SalesTwoYear { get; set; }
    public decimal? ProfitTwoYear { get; set; }

    public int? OrdersLastYear { get; set; }
    public int? UnitsLastYear { get; set; }
    public decimal? CostsLastYear { get; set; }
    public decimal? SalesLastYear { get; set; }
    public decimal? ProfitLastYear { get; set; }

    public int? OrdersThisYear { get; set; }
    public int? UnitsThisYear { get; set; }
    public decimal? CostsThisYear { get; set; }
    public decimal? SalesThisYear { get; set; }
    public decimal? ProfitThisYear { get; set; }

    public int? OrdersRolling { get; set; }
    public int? UnitsRolling { get; set; }
    public decimal? CostsRolling { get; set; }
    public decimal? SalesRolling { get; set; }
    public decimal? ProfitRolling { get; set; }
}