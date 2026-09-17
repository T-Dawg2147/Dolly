using System.Data;
using Dapper;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Data;

// CHANGED: now also takes IReportExportService, for reports that can't be expressed
// as a simple "SELECT * FROM X" (e.g. Supplier Report's two-step scalar SQL generation).
public sealed class GenericReportRunner(
    IConfiguration config,
    IExcelTemplateExportService excel,
    IReportExportService legacyReports) : IGenericReportRunner
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
                                  ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    public async Task RunAsync(
        ReportDefinition report, string outputFolder, string? supplierCode,
        bool reportingSupplier, IReadOnlyDictionary<string, object?>? extraParameters = null,
        CancellationToken ct = default)
    {
        // ADDED: CUSTOM reports delegate straight to the existing, already-working
        // IReportExportService methods instead of trying to build generic SQL for them.
        if (string.Equals(report.SourceKind, "CUSTOM", StringComparison.OrdinalIgnoreCase))
        {
            await RunCustomReportAsync(report, outputFolder, supplierCode, reportingSupplier, ct);
            return;
        }

        var (sql, parameters) = BuildSqlAndParameters(report, supplierCode, reportingSupplier, extraParameters);

        await using var conn = new SqlConnection(_cs);
        using var reader = await conn.ExecuteReaderAsync(new CommandDefinition(sql, parameters, cancellationToken: ct));

        var dt = new DataTable();
        dt.Load(reader);

        var title = report.TitleTemplate is null ? report.Title : string.Format(report.TitleTemplate, DateTime.Now);

        await excel.ExportAsync(report.OutputFileName, outputFolder,
        [
            new ExcelSheetExportSpec { SheetName = report.SheetName, StartCell = "A3", Title = title, Data = dt }
        ], ct);
    }

    // ADDED: routes specific catalog entries back to the hand-written service methods.
    // Match on Title here rather than magic strings scattered around — keeps this in one place.
    private async Task RunCustomReportAsync(
        ReportDefinition report, string outputFolder, string? supplierCode, bool reportingSupplier,
        CancellationToken ct)
    {
        switch (report.Title)
        {
            case "Supplier Report":
                if (string.IsNullOrWhiteSpace(supplierCode))
                    throw new InvalidOperationException("Supplier Report requires a selected supplier.");
                await legacyReports.ExportSupplierReportAsync(outputFolder, supplierCode, reportingSupplier, ct);
                break;

            case "Customer Sales By Supplier":
                if (string.IsNullOrWhiteSpace(supplierCode))
                    throw new InvalidOperationException("Customer Sales By Supplier requires a selected supplier.");
                await legacyReports.ExportCustomerSalesAsync(outputFolder, supplierCode, reportingSupplier, ct);
                break;

            default:
                throw new NotSupportedException(
                    $"Report '{report.Title}' is marked CUSTOM but has no matching handler in {nameof(GenericReportRunner)}.");
        }
    }

    private static (string sql, object? parameters) BuildSqlAndParameters(
        ReportDefinition report, string? supplierCode, bool reportingSupplier,
        IReadOnlyDictionary<string, object?>? extra)
    {
        // CHANGED: switch is now case-insensitive so catalog rows aren't broken by casing
        // (e.g. "date" vs "Date"), and RequiresReportingSupplierFlag is now respected.
        return report.ParameterMode.ToLowerInvariant() switch
        {
            "hierarchyleaf" => (
                $"EXEC {report.SourceObject} @LeafId",
                new { LeafId = extra?["LeafId"] }),

            "date" => (
                $"EXEC {report.SourceObject} @ForDate",
                new { ForDate = extra?["ForDate"] }),

            "supplliercontext" or "supplierContext" => // guard kept intentionally permissive
                BuildSupplierContextQuery(report, supplierCode, reportingSupplier),

            "none" => (
                $"SELECT * FROM {report.SourceObject}",
                null),

            _ => throw new NotSupportedException($"Unsupported ParameterMode '{report.ParameterMode}'")
        };
    }

    // ADDED: this is the piece that was missing entirely — reports that need to switch
    // between "Supplier_Code" and "Reporting_Supplier" behaviour based on the checkbox,
    // same as the VBA's `chkReportingSupplier` branch.
    private static (string sql, object? parameters) BuildSupplierContextQuery(
        ReportDefinition report, string? supplierCode, bool reportingSupplier)
    {
        if (!report.RequiresSupplierCode)
            return ($"SELECT * FROM {report.SourceObject}", null);

        if (!report.RequiresReportingSupplierFlag)
            return ($"SELECT * FROM {report.SourceObject}(@SupplierCode)", new { SupplierCode = supplierCode });

        // Mirrors the VBA:
        //   If Me.chkReportingSupplier = 0 Then
        //       SqlStr = "... fn_CustomerSalesBySupplier (@SupplierCode)"
        //   Else
        //       SqlStr = "... fn_CustomerSalesByReportingSupplier (@SupplierCode)"
        var functionName = reportingSupplier
            ? report.SourceObject.Replace("BySupplier", "ByReportingSupplier")
            : report.SourceObject;

        return ($"SELECT * FROM {functionName}(@SupplierCode)", new { SupplierCode = supplierCode });
    }
}