namespace Dolly.Application.Models;

public sealed class ProductEditRow
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string ObjectType { get; set; } = "";
    public string ParentId { get; set; } = "";

    public decimal? Carriage { get; set; }
    public decimal? CarriageCharge { get; set; }
    public string? CarriageType { get; set; }
    public decimal? CurrencyFactor { get; set; }

    public DateTime? DiscontinuedDate { get; set; }
    public string? DiscontinuedReason { get; set; }
    public decimal? FreeStock { get; set; }
    public decimal? FreightIn { get; set; }
    public decimal? FreightOut { get; set; }
    public string? GreenFlag { get; set; }
    public int? LeadTime { get; set; }

    public string? LongDescription1 { get; set; }
    public string? LongDescription2 { get; set; }

    public string? MadeToOrder { get; set; }
    public decimal? Margin { get; set; }
    public int? MinOrderQuantity { get; set; }
    public int? MinQty2 { get; set; }
    public int? MinQty3 { get; set; }
    public decimal? NetCostPrice2 { get; set; }
    public decimal? NetCostPrice3 { get; set; }

    public int? PageNumber { get; set; }

    public decimal? Price1 { get; set; }
    public decimal? Price2 { get; set; }
    public decimal? Price3 { get; set; }

    public string? ProductAlertMessage { get; set; }
    public DateTime? ProductMessageStartDate { get; set; }
    public DateTime? ProductMessageEndDate { get; set; }
    public DateTime? ReleaseDate { get; set; }

    public string? ReportingSupplier { get; set; }
    public decimal? StandardCost { get; set; }
    public string? StockStatus { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierCode { get; set; }
    public decimal? SupplierDiscount { get; set; }
    public decimal? SupplierListPrice { get; set; }
    public string? SupplierProductCode { get; set; }
    public string? SupplierProductDescription { get; set; }

    public decimal? TotalCost { get; set; }
    public string? WebProduct { get; set; }
    public decimal? Weight { get; set; }
    public string? Withdrawn { get; set; }

    public decimal? ZoneACarriage { get; set; }
    public decimal? ZoneBCarriage { get; set; }
    public decimal? ZoneCCarriage { get; set; }
    public decimal? ZoneDCarriage { get; set; }
    public decimal? ZoneECarriage { get; set; }
    public decimal? ZoneSCarriage { get; set; }

    public string? ProductMadeInUk { get; set; }
    public string? CountryOfOrigin { get; set; }

    public string? PackFlash { get; set; }
    public string? WebExclusive { get; set; }

    public decimal? PackagingHeight { get; set; }
    public decimal? PackagingLength { get; set; }
    public decimal? PackagingWidth { get; set; }

    public string? WebDeliveryIcon { get; set; }
    public string? TariffCode { get; set; }

    public string? RunToZero { get; set; }
    public DateTime? LastPushedDate { get; set; }

    public decimal? ActualLandedCost { get; set; }
    public DateTime? SupplierDueDate { get; set; }

    public string? DisplayRelated { get; set; }
    public decimal? AdditionalCosts { get; set; }
    public string? UpdateImages { get; set; }
    public string? UsePalletRate { get; set; }
    public int? NoOfPallets { get; set; }

    public string? Boxed { get; set; }
    public string? Ato { get; set; }

    public string? VideoUrl { get; set; }
    public string? VideoDescription { get; set; }

    public string? Transactional { get; set; }
    public string? NonTransactionalText { get; set; }

    public string? WebStores { get; set; }

    public decimal? UkPromoPrice { get; set; }
    public decimal? RoiPromoPrice { get; set; }

    public string? Component { get; set; }
    public string? AntiDumpingDuty { get; set; }

    public string? ShippingMethod { get; set; }
    public decimal? ShippingCosts { get; set; }

    public string? ManageStock { get; set; }

    public decimal? PromotionalCost { get; set; }
    public DateTime? PromotionStartDate { get; set; }
    public DateTime? PromotionEndDate { get; set; }

    public string? ProductSupplierMessage { get; set; }
    public string? WebDeliveryOverride { get; set; }
}

public sealed class ProductEditComputed
{
    public string? BaseCode { get; set; }
    public string? DespatchMethod { get; set; }
    public decimal? DespatchCost { get; set; }
    public decimal? SmallOrderValue { get; set; }
    public decimal? SmallOrderCharge { get; set; }
    public string? LocationPath { get; set; }
    public decimal? UkPromoPrice { get; set; }
    public decimal? RoiPromoPrice { get; set; }
}