using System.Data;
using ClosedXML.Excel;
using Dolly.Application.Abstraction;

namespace Dolly.Infrastructure.Export;

public sealed class ExcelTemplateExportService : IExcelTemplateExportService
{
    private readonly string _templateFolder;

    public ExcelTemplateExportService(string templateFolder)
    {
        _templateFolder = templateFolder;
    }

    public Task ExportAsync(
        string templateFileName,
        string outputFolder,
        IReadOnlyList<ExcelSheetExportSpec> sheets,
        CancellationToken ct = default)
    {
        if (sheets.Count == 0) return Task.CompletedTask;

        Directory.CreateDirectory(outputFolder);

        var templatePath = Path.Combine(_templateFolder, templateFileName);
        var outputPath = Path.Combine(outputFolder, templateFileName);

        if (File.Exists(outputPath))
            File.Delete(outputPath);

        File.Copy(templatePath, outputPath, overwrite: true);

        using var wb = new XLWorkbook(outputPath);

        for (var s = 0; s < sheets.Count; s++)
        {
            ct.ThrowIfCancellationRequested();

            var spec = sheets[s];
            var ws = wb.Worksheet(spec.SheetName);

            if (s == 0 && !string.IsNullOrWhiteSpace(spec.Title))
                ws.Cell("A1").Value = spec.Title;

            var start = ws.Cell(spec.StartCell);
            var startRow = start.WorksheetRow().RowNumber();
            var startCol = start.WorksheetColumn().ColumnNumber();
            var headerRow = startRow - 1;

            // Headers
            for (int i = 0; i < spec.Data.Columns.Count; i++)
            {
                ws.Cell(headerRow, startCol + i).Value = spec.Data.Columns[i].ColumnName;
                ws.Cell(headerRow, startCol + i).Style.Font.Bold = true;
            }

            // Data with type-aware assignment
            for (int r = 0; r < spec.Data.Rows.Count; r++)
            {
                for (int c = 0; c < spec.Data.Columns.Count; c++)
                {
                    var cell = ws.Cell(startRow + r, startCol + c);
                    var col = spec.Data.Columns[c];
                    var value = spec.Data.Rows[r][c];

                    SetTypedValue(cell, value, col.DataType);
                }
            }

            // Optional column trim
            if (spec.MaxColumns.HasValue && spec.MaxColumns.Value > 0)
            {
                var keepTo = spec.MaxColumns.Value;
                var lastUsed = ws.LastColumnUsed()?.ColumnNumber() ?? keepTo;
                if (lastUsed > keepTo)
                    ws.Columns(keepTo + 1, lastUsed).Delete();
            }

            // Nice usability defaults
            var used = ws.RangeUsed();
            if (used is not null)
            {
                used.SetAutoFilter();
                ws.Columns(used.FirstColumn().ColumnNumber(), used.LastColumn().ColumnNumber()).AdjustToContents();
                ws.SheetView.FreezeRows(headerRow);
            }
        }

        wb.Save();
        return Task.CompletedTask;
    }

    private static void SetTypedValue(IXLCell cell, object? rawValue, Type dataType)
    {
        if (rawValue is null || rawValue == DBNull.Value)
        {
            cell.Clear();
            return;
        }

        var t = Nullable.GetUnderlyingType(dataType) ?? dataType;

        if (t == typeof(string))
        {
            cell.Value = Convert.ToString(rawValue) ?? string.Empty;
            return;
        }

        if (t == typeof(DateTime))
        {
            var dt = Convert.ToDateTime(rawValue);
            cell.Value = dt;
            cell.Style.DateFormat.Format = "yyyy-mm-dd hh:mm";
            return;
        }

        if (t == typeof(bool))
        {
            cell.Value = Convert.ToBoolean(rawValue);
            return;
        }

        if (t == typeof(byte) || t == typeof(short) || t == typeof(int) || t == typeof(long))
        {
            cell.Value = Convert.ToInt64(rawValue);
            cell.Style.NumberFormat.Format = "0";
            return;
        }

        if (t == typeof(float) || t == typeof(double) || t == typeof(decimal))
        {
            cell.Value = Convert.ToDouble(rawValue);
            cell.Style.NumberFormat.Format = "0.########";
            return;
        }

        // Fallback
        cell.Value = Convert.ToString(rawValue) ?? string.Empty;
    }
}