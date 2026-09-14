using System.Data;

namespace Dolly.Application.Abstraction;

public interface IExcelTemplateExportService
{
    Task ExportAsync(
        string templateFileName,
        string outputFolder,
        IReadOnlyList<ExcelSheetExportSpec> sheets,
        CancellationToken ct = default);
}

public sealed class ExcelSheetExportSpec
{
    public string SheetName { get; init; } = "";
    public string StartCell { get; init; } = "A3";
    public string? Title { get; init; }
    public DataTable Data { get; init; } = new();
    public int? MaxColumns { get; init; }
}