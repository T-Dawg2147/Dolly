using System.Data;
using Dapper;
using DocumentFormat.OpenXml.Packaging;
using Dolly.Application.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Data;

/// <summary>
/// MIGRATION NOTE:
/// This version mirrors VBA report names and template sheet names.
/// </summary>
public sealed class ReportExportService(IConfiguration config, IExcelTemplateExportService excel) : IReportExportService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
        ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    private readonly IExcelTemplateExportService _excel = excel;

    public async Task ExportRunToZeroAsync(string folder, CancellationToken ct = default)
    {
        var dt = await QueryTableAsync("SELECT * FROM Catalogue.v_RunToZeroReport", ct);
        await _excel.ExportAsync("Run to Zero Report.xlsx", folder,
        [
            new ExcelSheetExportSpec
            {
                SheetName = "Run To Zero Report",
                StartCell = "A3",
                Title = $"Run to Zero Report - {DateTime.Now:D}",
                Data = dt
            }
        ], ct);
    }

    public async Task ExportSupplierStockAsync(string folder, CancellationToken ct = default)
    {
        var dt = await QueryTableAsync("SELECT * FROM Catalogue.fn_SupplierStockReport()", ct);
        await _excel.ExportAsync("Supplier Stock Report.xlsx", folder,
        [
            new ExcelSheetExportSpec
            {
                SheetName = "Supplier Stock Report",
                StartCell = "A3",
                Title = $"Stock Report by Section and Supplier - {DateTime.Now:D}",
                Data = dt
            }
        ], ct);
    }

    public async Task ExportSupplierReportAsync(string folder, string supplierCode, bool reportingSupplier,
        CancellationToken ct = default)
    {
        var supplierField = reportingSupplier ? "Reporting_Supplier" : "Supplier_Code";

        var generatedSql = await ScalarAsync<string>(
            "SELECT Catalogue.fn_SupplierReportData(@SupplierCode, @SupplierField) AS [SQL];",
            new { SupplierCode = supplierCode, SupplierField = supplierField },
            ct);

        if (string.IsNullOrWhiteSpace(generatedSql))
            throw new InvalidOperationException("Supplier report SQL generator return no SQL.");
        
        var dt = await QueryTableAsync(generatedSql, ct);

        await _excel.ExportAsync("Supplier Report.xlsx", folder,
        [
            new ExcelSheetExportSpec
            {
                SheetName = "Supplier Report",
                StartCell = "A3",
                Title = $"Supplier Report for {supplierCode} {DateTime.Now:MMMM yyyy}",
                Data = dt
            }
        ], ct);
    }

    public async Task ExportCustomerSalesAsync(string folder, string supplierCode, bool reportingSupplier,
        CancellationToken ct = default)
    {
        var sql = reportingSupplier
            ? "SELECT * FROM Sales.fn_CustomerSalesByReportingSupplier (@SupplierCode)"
            : "SELECT * FROM Sales.fn_CustomerSalesBySupplier (@SupplierCode)";

        var dt = await QueryTableAsync(sql, ct, new { SupplierCode = supplierCode });

        await _excel.ExportAsync("Customer Sales By Supplier.xlsx", folder,
        [
            new ExcelSheetExportSpec
            {
                SheetName = "Customer Report",
                StartCell = "A3",
                Title = $"Customer Sales Report {supplierCode} {DateTime.Now:MMMM yyyy}",
                Data = dt
            }
        ], ct);
    }

    public async Task ExportPromoPricingAsync(string folder, CancellationToken ct = default)
    {
        var dt = await QueryTableAsync("SELECT * FROM Catalogue.fn_PromoPricingReport()", ct);
        await _excel.ExportAsync("Promo Pricing Report.xlsx", folder,
        [
            new ExcelSheetExportSpec
            {
                SheetName = "Promo Pricing",
                StartCell = "A3",
                Title = $"Promo Pricing Report {DateTime.Now:MMMM yyyy}",
                Data = dt
            }
        ], ct);
    }

    public async Task ExportWebRankingAsync(string folder, CancellationToken ct = default)
    {
        var dt = await QueryTableAsync("SELECT * FROM Catalogue.fn_WebRankingByCategory()", ct);
        await _excel.ExportAsync("Web Rankings By Category.xlsx", folder,
        [
            new ExcelSheetExportSpec
            {
                SheetName = "Web Ranking By Category",
                StartCell = "A3",
                Title = $"Web Ranking By Category Report - {DateTime.Now:D}",
                Data = dt
            }
        ], ct);
    }

    public async Task ExportZeroSalesAsync(string folder, CancellationToken ct = default)
    {
        var dt = await QueryTableAsync("SELECT * FROM Sales.ZeroSales3Years", ct);
        await _excel.ExportAsync("Zero Sales 3 Year Report.xlsx", folder,
        [
            new ExcelSheetExportSpec
            {
                SheetName = "Sheet1",
                StartCell = "A3",
                Title = $"Products with No Sales in Last 3 Years - {DateTime.Now:D}",
                Data = dt
            }
        ], ct);
    }

    private async Task<DataTable> QueryTableAsync(string sql, CancellationToken ct, object? args = null)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(sql, conn);
        if (args is not null)
        {
            foreach (var p in args.GetType().GetProperties())
                cmd.Parameters.AddWithValue($"@{p.Name}", p.GetValue(args) ?? DBNull.Value);
        }

        await using var rdr = await cmd.ExecuteReaderAsync(ct);
        var dt = new DataTable();
        dt.Load(rdr);
        return dt;
    }

    private async Task ExecAsync(string sql, object args, CancellationToken ct)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.ExecuteAsync(new CommandDefinition(sql, args, cancellationToken: ct));
    }

    private async Task<T?> ScalarAsync<T>(string sql, object args, CancellationToken ct)
    {
        await using var conn = new SqlConnection(_cs);
        return await conn.ExecuteScalarAsync<T>(new CommandDefinition(sql, args, cancellationToken: ct));
    }
}