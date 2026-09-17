namespace Dolly.Application.Models;

public sealed class ReportDefinition
{
    public int ReportId { get; set; }
    public string Category { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string SourceObject { get; set; } = "";
    public string SourceKind { get; set; } = "VIEW"; // VIEW | TABLE_FUNCTION | SCALAR_SQL_FUNCTION
    public bool RequiresSupplierCode { get; set; }
    public bool RequiresReportingSupplierFlag { get; set; }
    public string ParameterMode { get; set; } = "None";
    public string OutputFileName { get; set; } = "";
    public string SheetName { get; set; } = "";
    public string? TitleTemplate { get; set; }
    public int SortOrder { get; set; }
}