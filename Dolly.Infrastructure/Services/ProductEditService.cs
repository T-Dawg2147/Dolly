using Dapper;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dolly.Infrastructure.Services;

public sealed class ProductEditService(IConfiguration config) : IProductEditService, IProductQueriesService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
                                  ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    public async Task<ProductEditRow?> LoadAsync(string designNo)
    {
        const string sql = """
           SELECT
               [Id] AS Id,[Name] AS Name,[Object_Type] AS ObjectType,[Parent_ID] AS ParentId,
               [Carriage] AS Carriage,[Carriage_Charge] AS CarriageCharge,[Carriage_Type] AS CarriageType,[Currency_Factor] AS CurrencyFactor,
               [Discontinued_Date] AS DiscontinuedDate,[Discontinued_Reason] AS DiscontinuedReason,[FreeStock] AS FreeStock,
               [Freight_In] AS FreightIn,[Freight_Out] AS FreightOut,[Green_Flag] AS GreenFlag,[Lead_Time] AS LeadTime,
               [Long_Description_1] AS LongDescription1,[Long_Description_2] AS LongDescription2,[Made_To_Order] AS MadeToOrder,
               [Margin] AS Margin,[Min_Order_Quantity] AS MinOrderQuantity,[MinQty2] AS MinQty2,[MinQty3] AS MinQty3,
               [NetCostPrice2] AS NetCostPrice2,[NetCostPrice3] AS NetCostPrice3,[Page_Number] AS PageNumber,
               [Price_1] AS Price1,[Price_2] AS Price2,[Price_3] AS Price3,[Product_Alert_Message] AS ProductAlertMessage,
               [Product_Message_Start_Date] AS ProductMessageStartDate,[Product_Message_End_Date] AS ProductMessageEndDate,
               [Release_Date] AS ReleaseDate,[Reporting_Supplier] AS ReportingSupplier,[Standard_Cost] AS StandardCost,
               [Stock_Status] AS StockStatus,[Supplier_Code] AS SupplierCode,[Supplier_Discount] AS SupplierDiscount,
               [Supplier_List_Price] AS SupplierListPrice,[Supplier_Product_Code] AS SupplierProductCode,
               [Supplier_Product_Description] AS SupplierProductDescription,[Total_Cost] AS TotalCost,[Web_Product] AS WebProduct,
               [Weight] AS Weight,[Withdrawn] AS Withdrawn,[Zone_A_Carriage] AS ZoneACarriage,[Zone_B_Carriage] AS ZoneBCarriage,
               [Zone_C_Carriage] AS ZoneCCarriage,[Zone_D_Carriage] AS ZoneDCarriage,[Zone_E_Carriage] AS ZoneECarriage,
               [Zone_S_Carriage] AS ZoneSCarriage,[Product_Made_In_UK] AS ProductMadeInUk,[Country_of_Origin] AS CountryOfOrigin,
               [Pack_Flash] AS PackFlash,[Web_Exclusive] AS WebExclusive,[Packaging_Height] AS PackagingHeight,
               [Packaging_Length] AS PackagingLength,[Packaging_Width] AS PackagingWidth,[Web_Delivery_Icon] AS WebDeliveryIcon,
               [Tariff_Code] AS TariffCode,[Run_To_Zero] AS RunToZero,[Last_Pushed_Date] AS LastPushedDate,
               [Actual_Landed_Cost] AS ActualLandedCost,[Supplier_Due_Date] AS SupplierDueDate,[Display_Related] AS DisplayRelated,
               [Additional_Costs] AS AdditionalCosts,[Update_Images] AS UpdateImages,[Use_Pallet_Rate] AS UsePalletRate,
               [No_Of_Pallets] AS NoOfPallets,[Boxed] AS Boxed,[ATO] AS Ato,[Video_URL] AS VideoUrl,[Video_Description] AS VideoDescription,
               [Transactional] AS Transactional,[Non_Transactional_Text] AS NonTransactionalText,[Web_Stores] AS WebStores,
               [UK_Promo_Price] AS UkPromoPrice,[ROI_Promo_Price] AS RoiPromoPrice,[Component] AS Component,[Anti_Dumping_Duty] AS AntiDumpingDuty,
               [Shipping_Method] AS ShippingMethod,[Shipping_Costs] AS ShippingCosts,[Manage_Stock] AS ManageStock,
               [Promotional_Cost] AS PromotionalCost,[Promotion_Start_Date] AS PromotionStartDate,[Promotion_End_Date] AS PromotionEndDate,
               [Product_Supplier_Message] AS ProductSupplierMessage,[Web_Delivery_Override] AS WebDeliveryOverride
           FROM Catalogue.WorkingTable
           WHERE [Id] = @Id OR [Name] = @Id;
           """;

        await using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<ProductEditRow>(sql, new { Id = designNo });
    }

    public async Task<SaveProductEditResult> SaveAsync(ProductEditRow row)
    {
        try
        {
            const string sql = """
               UPDATE Catalogue.WorkingTable
               SET
                   Object_Type=@ObjectType, Parent_ID=@ParentId, Carriage_Charge=@CarriageCharge, Carriage_Type=@CarriageType,
                   Currency_Factor=@CurrencyFactor, Discontinued_Date=@DiscontinuedDate, Discontinued_Reason=@DiscontinuedReason,
                   Freight_In=@FreightIn, Freight_Out=@FreightOut, Green_Flag=@GreenFlag, Lead_Time=@LeadTime,
                   Long_Description_1=@LongDescription1, Long_Description_2=@LongDescription2, Made_To_Order=@MadeToOrder,
                   Margin=@Margin, Min_Order_Quantity=@MinOrderQuantity, MinQty2=@MinQty2, MinQty3=@MinQty3,
                   NetCostPrice2=@NetCostPrice2, NetCostPrice3=@NetCostPrice3, Page_Number=@PageNumber,
                   Price_1=@Price1, Price_2=@Price2, Price_3=@Price3, Product_Alert_Message=@ProductAlertMessage,
                   Product_Message_Start_Date=@ProductMessageStartDate, Product_Message_End_Date=@ProductMessageEndDate,
                   Release_Date=@ReleaseDate, Standard_Cost=@StandardCost, Stock_Status=@StockStatus, Supplier_Code=@SupplierCode,
                   Supplier_Discount=@SupplierDiscount, Supplier_List_Price=@SupplierListPrice, Supplier_Product_Code=@SupplierProductCode,
                   Total_Cost=@TotalCost, Web_Product=@WebProduct, Weight=@Weight, Withdrawn=@Withdrawn,
                   Zone_A_Carriage=@ZoneACarriage, Zone_B_Carriage=@ZoneBCarriage, Zone_C_Carriage=@ZoneCCarriage,
                   Zone_D_Carriage=@ZoneDCarriage, Zone_E_Carriage=@ZoneECarriage, Zone_S_Carriage=@ZoneSCarriage,
                   Product_Made_In_UK=@ProductMadeInUk, Country_of_Origin=@CountryOfOrigin, Pack_Flash=@PackFlash,
                   Web_Exclusive=@WebExclusive, Packaging_Height=@PackagingHeight, Packaging_Length=@PackagingLength,
                   Packaging_Width=@PackagingWidth, Web_Delivery_Icon=@WebDeliveryIcon, Tariff_Code=@TariffCode,
                   Run_To_Zero=@RunToZero, Last_Pushed_Date=@LastPushedDate, Actual_Landed_Cost=@ActualLandedCost,
                   Supplier_Due_Date=@SupplierDueDate, Display_Related=@DisplayRelated, Additional_Costs=@AdditionalCosts,
                   Update_Images=@UpdateImages, Use_Pallet_Rate=@UsePalletRate, No_Of_Pallets=@NoOfPallets,
                   Boxed=@Boxed, ATO=@Ato, Video_URL=@VideoUrl, Video_Description=@VideoDescription,
                   Transactional=@Transactional, Non_Transactional_Text=@NonTransactionalText, Web_Stores=@WebStores,
                   UK_Promo_Price=@UkPromoPrice, ROI_Promo_Price=@RoiPromoPrice, Component=@Component,
                   Anti_Dumping_Duty=@AntiDumpingDuty, Shipping_Method=@ShippingMethod, Shipping_Costs=@ShippingCosts,
                   Manage_Stock=@ManageStock, Promotional_Cost=@PromotionalCost, Promotion_Start_Date=@PromotionStartDate,
                   Promotion_End_Date=@PromotionEndDate, Product_Supplier_Message=@ProductSupplierMessage, Web_Delivery_Override=@WebDeliveryOverride
               WHERE Id=@Id;
               """;

            await using var conn = new SqlConnection(_cs);
            await conn.ExecuteAsync(sql, row);
            return new SaveProductEditResult { Success = true };
        }
        catch (Exception ex)
        {
            return new SaveProductEditResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<SaveProductEditResult> PersistPalletFieldsAsync(string designNo, bool usePalletRate, int? noOfPallets)
    {
        try
        {
            const string sql = """
                UPDATE Catalogue.WorkingTable
                SET Use_Pallet_Rate=@UsePalletRate, No_Of_Pallets=@NoOfPallets
                WHERE Name=@Code OR Id=@Code;
                """;

            await using var conn = new SqlConnection(_cs);
            await conn.ExecuteAsync(sql, new
            {
                Code = designNo,
                UsePalletRate = usePalletRate ? "Yes" : "No",
                NoOfPallets = noOfPallets ?? 0
            });

            return new SaveProductEditResult { Success = true };
        }
        catch (Exception ex)
        {
            return new SaveProductEditResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<ApplyChangesResult> ApplyWorkingTableAndCommitAsync(ProductEditRow dto, string username)
    {
        try
        {
            await using var conn = new SqlConnection(_cs);
            await conn.OpenAsync();
            await using var tx = await conn.BeginTransactionAsync();

            var p = new DynamicParameters();
            p.Add("@SupplierCode", "%");
            p.Add("@ProductCode", dto.Name);
            p.Add("@Username", username);
            p.Add("@NoRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("catalogue.usp_UpdateStepHistory", p, tx, commandType: CommandType.StoredProcedure);
            await conn.ExecuteAsync("catalogue.usp_UpdateProductTable", new { Username = username }, tx, commandType: CommandType.StoredProcedure);
            await conn.ExecuteAsync("catalogue.usp_ExportToSTEP", new { ObjectType = dto.ObjectType, Username = username }, tx, commandType: CommandType.StoredProcedure);

            await tx.CommitAsync();
            return new ApplyChangesResult { Success = true };
        }
        catch (Exception ex)
        {
            return new ApplyChangesResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<IReadOnlyList<ProductHistoryRow>> GetHistoryAsync(string designNo)
    {
        const string sql = """
            SELECT [Attribute] AS Attribute, [Old Value] AS OldValue, [New Value] AS NewValue
            FROM Catalogue.fn_ProductHistory(@DesignNo);
            """;
        await using var conn = new SqlConnection(_cs);
        var rows = await conn.QueryAsync<ProductHistoryRow>(sql, new { DesignNo = designNo });
        return rows.AsList();
    }

    public async Task<IReadOnlyList<string>> GetSupplierNamesAsync() => await QueryStringListAsync("SELECT Supplier_Name FROM Catalogue.Suppliers ORDER BY Supplier_Name;");
    public async Task<IReadOnlyList<string>> GetCountryOptionsAsync() => await QueryStringListAsync("SELECT DISTINCT CountryOfOrigin FROM Catalogue.CountryOfOrigin ORDER BY CountryOfOrigin;");
    public async Task<IReadOnlyList<string>> GetWebOverlayOptionsAsync() => await QueryStringListAsync("SELECT DISTINCT Pack_Flash FROM Catalogue.WorkingTable WHERE Pack_Flash IS NOT NULL ORDER BY Pack_Flash;");
    public async Task<IReadOnlyList<string>> GetWebExclusiveOptionsAsync() => await QueryStringListAsync("SELECT DISTINCT Web_Exclusive FROM Catalogue.WorkingTable WHERE Web_Exclusive IS NOT NULL ORDER BY Web_Exclusive;");
    public async Task<IReadOnlyList<string>> GetWebDeliveryOptionsAsync() => await QueryStringListAsync("SELECT DISTINCT Name FROM Catalogue.WebDeliveryValues ORDER BY Name;");
    public async Task<IReadOnlyList<string>> GetDiscontinuedReasonOptionsAsync() => await QueryStringListAsync("SELECT TextAnswer FROM Catalogue.RelatedData WHERE UserTypeId = 1 ORDER BY TextAnswer;");

    public Task<RuleEvaluationResult> EvaluateRulesAsync(ProductEditRow dto) => Task.FromResult(new RuleEvaluationResult { Success = true });

    public async Task<RepriceResult> RepriceAsync(ProductEditRow dto)
    {
        var cost = await RecalculateCostingAsync(dto.CarriageCharge, dto.CurrencyFactor, dto.CarriageType, dto.FreightOut, dto.StandardCost, dto.StockStatus, dto.Weight, dto.Name);
        return new RepriceResult
        {
            Success = true,
            NewDespatchCost = cost?.DespatchCost,
            NewTotalCost = cost?.TotalCost,
            NewSellingPrice1 = dto.Price1,
            NewMargin = dto.Margin
        };
    }

    public async Task<OverlayUpdateResult> UpdateOverlayAsync(string designNo, string? webOverlay, string? webExclusive)
    {
        try
        {
            const string sql = "UPDATE Catalogue.WorkingTable SET Pack_Flash=@WebOverlay, Web_Exclusive=@WebExclusive WHERE Name=@Code OR Id=@Code;";
            await using var conn = new SqlConnection(_cs);
            await conn.ExecuteAsync(sql, new { Code = designNo, WebOverlay = webOverlay, WebExclusive = webExclusive });
            return new OverlayUpdateResult { Success = true };
        }
        catch (Exception ex)
        {
            return new OverlayUpdateResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<PushToMagentoResult> PushToMagentoAsync(string designNo)
    {
        try
        {
            await using var conn = new SqlConnection(_cs);
            await conn.ExecuteAsync("catalogue.usp_PushToMagento", new { DesignNo = designNo }, commandType: CommandType.StoredProcedure);
            return new PushToMagentoResult { Success = true };
        }
        catch (Exception ex)
        {
            return new PushToMagentoResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public Task<ProductEditRow?> GetProductEditAsync(string productId, CancellationToken ct = default) => LoadAsync(productId);
    public Task<IReadOnlyList<ProductHistoryRow>> GetProductHistoryAsync(string productIdOrName, CancellationToken ct = default) => GetHistoryAsync(productIdOrName);

    public async Task<ProductCostingSnapshotRow?> RecalculateCostingAsync(decimal? carriageCharge, decimal? currencyFactor, string? carriageType, decimal? freightOut, decimal? standardCost, string? stockStatus, decimal? weight, string? codeOrName, CancellationToken ct = default)
    {
        const string sql = """
            SELECT TOP 1 C.Carriage AS Carriage, @StandardCost AS StandardCost, NULL AS TotalCost, D.DespatchMethod, D.DespatchCost
            FROM Catalogue.fn_Carriage(@CarriageCharge,@CurrencyFactor,@CarriageType,@FreightOut,@StandardCost,@StockStatus,@Weight) C
            CROSS APPLY Catalogue.fn_DespatchMethod2(@CodeOrName,@StockStatus) D;
            """;

        await using var conn = new SqlConnection(_cs);
        var row = await conn.QueryFirstOrDefaultAsync<ProductCostingSnapshotRow>(sql, new
        {
            CarriageCharge = carriageCharge ?? 0m,
            CurrencyFactor = currencyFactor ?? 0m,
            CarriageType = carriageType ?? "",
            FreightOut = freightOut ?? 0m,
            StandardCost = standardCost ?? 0m,
            StockStatus = stockStatus ?? "",
            Weight = weight ?? 0m,
            CodeOrName = codeOrName ?? ""
        });

        if (row is not null) row.TotalCost = (row.StandardCost ?? 0m) + (row.Carriage ?? 0m);
        return row;
    }

    public async Task<string?> GetProductImagePathAsync(string codeOrName, CancellationToken ct = default)
    {
        const string sql = "SELECT TOP 1 Catalogue.fn_STEPImagePath(@CodeOrName);";
        await using var conn = new SqlConnection(_cs);
        return await conn.ExecuteScalarAsync<string?>(sql, new { CodeOrName = codeOrName });
    }

    public async Task<ProductAuxLookups> GetAuxLookupsAsync(string codeOrName, string? stockStatus, CancellationToken ct = default)
    {
        const string sql = """
           SELECT TOP 1 D.DespatchMethod, D.DespatchCost, S.OrderValue AS SmallOrderValue, S.OrderCharge AS SmallOrderCharge
           FROM Catalogue.fn_DespatchMethod2(@Code,@StockStatus) D CROSS JOIN Catalogue.SmallOrderCharges S;
           SELECT CONCAT('Location: ',COALESCE(N.Section,''),'/',COALESCE(N.Subsection1,'')) AS LocationPath
           FROM Catalogue.StepNames N WHERE N.Name=@Code;
           SELECT TOP 1 CAST(P.Price1UK AS nvarchar(50)) AS UkPromoPriceText, CAST(P.Price1ROI AS nvarchar(50)) AS RoiPromoPriceText
           FROM Catalogue.fn_ProductPromoPricing(@Code) P;
           """;

        await using var conn = new SqlConnection(_cs);
        await using var multi = await conn.QueryMultipleAsync(sql, new { Code = codeOrName, StockStatus = stockStatus ?? "" });

        var lookup = await multi.ReadFirstOrDefaultAsync<ProductAuxLookups>() ?? new ProductAuxLookups();
        var loc = await multi.ReadFirstOrDefaultAsync<ProductAuxLookups>();
        var promo = await multi.ReadFirstOrDefaultAsync<ProductAuxLookups>();

        lookup.LocationPath = loc?.LocationPath;
        lookup.UkPromoPriceText = promo?.UkPromoPriceText;
        lookup.RoiPromoPriceText = promo?.RoiPromoPriceText;
        return lookup;
    }

    public async Task<bool> HasRelatedProductsAsync(string codeOrName, CancellationToken ct = default)
    {
        const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Catalogue.RelatedProductsByRef WHERE Id=@Code AND Unlink=0) THEN CAST(1 as bit) ELSE CAST(0 as bit) END;";
        await using var conn = new SqlConnection(_cs);
        return await conn.ExecuteScalarAsync<bool>(sql, new { Code = codeOrName });
    }

    public async Task<bool> HasDualFeatureChildrenAsync(string codeOrName, CancellationToken ct = default)
    {
        const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Catalogue.DualFeatured WHERE Overrides=@Code AND Object_Type='Dual_Featured_Product') THEN CAST(1 as bit) ELSE CAST(0 as bit) END;";
        await using var conn = new SqlConnection(_cs);
        return await conn.ExecuteScalarAsync<bool>(sql, new { Code = codeOrName });
    }

    public async Task MarkUpdateImagesAsync(string codeOrName, CancellationToken ct = default)
    {
        const string sql = "UPDATE Catalogue.WorkingTable SET Update_Images='Yes' WHERE Name=@Code OR Id=@Code;";
        await using var conn = new SqlConnection(_cs);
        await conn.ExecuteAsync(sql, new { Code = codeOrName });
    }

    public async Task<ProductEditComputed> LoadComputedAsync(string codeOrName, string? stockStatus, decimal? carriageCharge, decimal? currencyFactor,
        string? carriageType, decimal? freightOut, decimal? standardCost, decimal? weight, CancellationToken ct = default)
    {
        const string sql = """
           SELECT
            P.Base_Code AS BaseCode
           FROM Catalogue.Products P;
           
           -- Despatch + small order values
           SELECT TOP 1
            D.DespatchMethod,
            D.DespatchCost,
            S.OrderValue AS SmallOrderValue,
            S.OrderCharge AS SmallOrderCharge
           FROM Catalogue.fn_DespatchMethod2(@CodeOrName, @StockStatus) D
           CROSS JOIN Catalogue.SmallOrderCharges S;
           
           -- Step Path
           SELECT TOP 1
            CONCAT(
                'Location: ',
                COALESCE(N.Section, ''), '/',
                COALESCE(N.Subsection1, ''),
                CASE WHEN COALESCE(N.Subsection2, '') = '' THEN '' ELSE CONCAT('/', N.Subsection2) END,
                CASE WHEN COALESCE(N.Subsection3, '') = '' THEN '' ELSE CONCAT('/', N.Subsection3) END
            ) AS LocationPath
           FROM Catalogue.StepNames N
           WHERE N.Name = @CodeOrName;
           
           -- Promo pricing
           SELECT TOP 1
            CAST(P.Price1UK AS decimal(10, 2)) AS UkPromoPrice,
            CAST(P.Price1ROI AS decimal(10, 2)) AS RoiPromoPrice
           FROM Catalogue.fn_ProductPromoPricing(@CodeOrName) P;
           
           -- Carriage Calc
           SELECT TOP 1
               C.Carriage AS Carriage
           FROM Catalogue.fn_Carriage(
               @CarriageCharge,
               @CurrencyFactor,
               @CarriageType,
               @FreightOut,
               @StandardCost,
               @StockStatus,
               @Weight
           ) C;
           """;

        await using var conn = new SqlConnection(_cs);
        await using var multi = await conn.QueryMultipleAsync(new CommandDefinition(sql, new
        {
            CodeOrName = codeOrName,
            StockStatus = stockStatus ?? "",
            CarriageCharge = carriageCharge ?? 0m,
            CurrencyFactor = currencyFactor ?? 0m,
            carriageType = carriageType ?? "",
            FreightOut = freightOut ?? 0m,
            StandardCost = standardCost ?? 0m,
            Weight = weight ?? 0m
        }, cancellationToken: ct));

        var result = await multi.ReadFirstOrDefaultAsync<ProductEditComputed>() ?? new ProductEditComputed();
        var loc = await multi.ReadFirstOrDefaultAsync<ProductEditComputed>();
        var promo = await multi.ReadFirstOrDefaultAsync<ProductEditComputed>();
        _ = await multi.ReadFirstOrDefaultAsync<dynamic>();

        result.LocationPath = loc?.LocationPath;
        result.UkPromoPrice = promo?.UkPromoPrice;
        result.RoiPromoPrice = promo?.RoiPromoPrice;

        return result;
    }

    private async Task<IReadOnlyList<string>> QueryStringListAsync(string sql)
    {
        await using var conn = new SqlConnection(_cs);
        var rows = await conn.QueryAsync<string>(sql);
        return rows.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}