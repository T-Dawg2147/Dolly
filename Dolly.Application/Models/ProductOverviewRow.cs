namespace Dolly.Application.Models;

// TODO Fix data types and reorder
public sealed class ProductOverviewRow
{
    public string Description { get; set; } = "";
    public string PartNo { get; set; } = "";
    public string StockStatus { get; set; } = "";
    public string StockControl { get; set; } = "";
    public string NextDay { get; set; } = "";
    public string RunToZero { get; set; } = "";
    public decimal ListPrice { get; set; }
    public string Discount { get; set; } = "";
    public decimal NetCost { get; set; }
    public string LandedCost { get; set; } = "";
    public string Carriage { get; set; } = "";
    public decimal AdditionalCosts { get; set; }
    public decimal TotalCost { get; set; }
    public decimal SellingPriceGbp { get; set; }
    public decimal Margin { get; set; }
    public int Moq { get; set; }
    public int LeadTime { get; set; }
    public string DateIntoStock { get; set; } = ""; // DateTime
    public int StockCost { get; set; }
    public int AvailableStock { get; set; }
    public int BackOrders { get; set; }
    public YearToDateSales YearToDateSales { get; set; } = new();
    public string ReleaseDate { get; set; } = "";
    public decimal SellingPriceEuro { get; set; }
    public string SocMargin { get; set; } = "";
    public string SalesMessage { get; set; } = "";
}

// TODO might make a record instead of a class? It wont have any logic, just values
public sealed class YearToDateSales
{
    // need to translate Year, Year - 1, Year - 2 as well as the units and the sales for that into properties... Just not sure what to name them as they are dynamic to the current year.
}