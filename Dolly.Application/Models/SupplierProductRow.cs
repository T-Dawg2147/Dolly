namespace Dolly.Application.Models;

public sealed class SupplierProductRow
{
    public string? SupplierCode { get; set; }
    public string? Section { get; set; }
    public string? ProductManager { get; set; }
    public int? PageNo { get; set; }
    public int? OldPageNo { get; set; }
    public string? Pgrp { get; set; }
    public string? BaseCode { get; set; }
    public string? SupplierProductCode { get; set; }
    public string? DesignNo { get; set; }
    public string? Parent { get; set; }
    public string? Description1 { get; set; }
    public string? Description2 { get; set; }
    public string? StockStatus { get; set; }
    public string? StockControl { get; set; }
    public string? NextDay { get; set; } // bool
    public string? RunToZero { get; set; } // bool

    public decimal? Stock { get; set; }
    public decimal? OnOrder { get; set; }
    public decimal? BackOrders { get; set; }
    public int? LeadTime { get; set; }

    public decimal? ListPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? NetCost { get; set; }
    public decimal? NetCost2 { get; set; }
    public decimal? NetCost3 { get; set; }
    public decimal? MinQty2 { get; set; }
    public decimal? MinQty3 { get; set; }

    public decimal? CurrencyRate { get; set; }
    public decimal? FreightIn { get; set; }
    public decimal? SettlementDisc { get; set; }
    public decimal? StandardCost { get; set; }

    public string? CarriageType { get; set; }
    public decimal? FreightOut { get; set; }
    public decimal? CarriageCharge { get; set; }
    public string? DespatchMethod { get; set; }
    public decimal? Carriage { get; set; }
    public decimal? AdditionalCosts { get; set; }
    public decimal? TotalCost { get; set; }

    public decimal? Price1 { get; set; }
    public decimal? Price2 { get; set; }
    public decimal? Price3 { get; set; }
    public decimal? PriceBreak2 { get; set; }
    public decimal? PriceBreak3 { get; set; }

    public decimal? Margin { get; set; }
    public decimal? SmallMargin { get; set; }

    public decimal? ZoneA { get; set; }
    public decimal? ZoneB { get; set; }
    public decimal? ZoneC { get; set; }
    public decimal? ZoneD { get; set; }
    public decimal? ZoneE { get; set; }
    public decimal? ZoneS { get; set; }
}