namespace Dolly.Application.Models;

public sealed class SaveProductEditResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class ApplyChangesResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class ProductHistoryRow
{
    public string? Attribute { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime? ChangedDate { get; set; }
}

public sealed class RuleEvaluationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public bool ForceRunToZero { get; set; }
    public bool ForceTransactionalOff { get; set; }
}

public sealed class RepriceResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? NewDespatchCost { get; set; }
    public decimal? NewTotalCost { get; set; }
    public decimal? NewSellingPrice1 { get; set; }
    public decimal? NewSellingPrice2 { get; set; }
    public decimal? NewSellingPrice3 { get; set; }
    public decimal? NewMargin { get; set; }
    public decimal? NewSoMargin { get; set; }
}

public sealed class OverlayUpdateResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class PushToMagentoResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}