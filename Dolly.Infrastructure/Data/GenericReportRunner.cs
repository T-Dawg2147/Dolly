using System.Data;
using Dapper;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Data;

public sealed class GenericReportRunner(IConfiguration config, IExcelTemplateExportService excel)
    : IGenericReportRunner
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
                                  ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    // TODO: Gotta go through this below and double check this will all work with ever report type
    public async Task RunAsync(
        ReportDefinition report, string outputFolder, string? supplierCode,
        bool reportingSupplier, IReadOnlyDictionary<string, object?>? extraParameters = null,
        CancellationToken ct = default)
    {
        var (sql, parameters) = BuildSqlAndParameters(report, supplierCode, extraParameters);

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

    private static (string sql, object? parameters) BuildSqlAndParameters(ReportDefinition report, string? supplierCode, IReadOnlyDictionary<string, object?>? extra)
    {
        return report.ParameterMode switch
        {
            "HierarchyLeaf" => (
                $"EXEC {report.SourceObject} @LeafId",
                new { LeafId = extra?["LeafId"]}),
            
            "Date" => (
                $"EXEC {report.SourceObject} @ForDate",
                new { ForDate = extra?["ForDate"] }),
            
            "SupplierContext" when report.RequiresSupplierCode => (
                $"SELECT * FROM {report.SourceObject}(@SupplierCode)",
                new { SupplierCode = supplierCode}),
            
            "None" or "SupplierContext" => (
                $"SELECT * FROM {report.SourceObject}",
                null),
            
            _ => throw new NotSupportedException($"Unsupported ParameterMode '{report.ParameterMode}'")
        };
    }
}