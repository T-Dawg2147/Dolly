using Dapper;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Data;

public sealed class SupplierQueriesService(IConfiguration config) : ISupplierQueriesService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
                                  ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    public async Task<IReadOnlyList<SupplierSummary>> SearchSuppliersAsync(string term, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT
                            s.Supplier_Code AS SupplierCode,
                            s.Supplier_Name AS SupplierName
                           FROM Catalogue.Suppliers AS s
                           WHERE (@term = '' OR s.Supplier_Code LIKE '%' + @term + '%' OR s.Supplier_Name LIKE '%' + @term + '%')
                           ORDER BY s.Supplier_Name;
                           """;

        await using var conn = new SqlConnection(_cs);
        var rows = await conn.QueryAsync<SupplierSummary>(new CommandDefinition(sql, new { term = term ?? "" },
            cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<SupplierDetailsRow?> GetSupplierDetailsAsync(string supplierCode, CancellationToken ct = default)
    {
        const string sql = """
           SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
           
           SELECT
               -- Suppliers
               S.Supplier_Code AS SupplierCode,
               S.Supplier_Name AS SupplierName,
               S.Full_Name AS FullName,
               S.Address_Line1 AS AddressLine1,
               S.Address_Line2 AS AddressLine2,
               S.Address_Line3 AS AddressLine3,
               S.Address_Line4 AS AddressLine4,
               S.Address_Line5 AS AddressLine5,
               S.Country AS Country,
               S.Post_Code AS PostCode,
               S.Telephone_No AS TelephoneNo,
               S.Facsimile_No AS FacsimileNo,
               S.Website AS Website,
               S.General_Email AS GeneralEmail,
               S.Buyer AS Buyer,
               S.Currency AS Currency,
               S.Current_Status AS CurrentStatus,
               S.Discontinued_Reason AS DiscontinuedReason,
               S.EU_Country AS EuCountry,
               S.Has_File AS HasFile,
               S.Imported AS Imported,
               S.Products_Conform_Legislation AS ProductsConformLegislation,
               S.Send_Questionnaire AS SendQuestionnaire,
               S.Supplier_Message AS SupplierMessage,
               S.WEEE_Reg AS WeeeReg,
               S.Pricing_Start_Month AS PricingStartMonth,
               S.Last_Used AS LastUsed,
               S.Quality_System AS QualitySystem,
               S.Environmental_System AS EnvironmentalSystem,
               S.Health_Safety_System AS HealthSafetySystem,
               S.Accounts_Contact AS AccountsContact,
               S.Accounts_Contact_Email AS AccountsContactEmail,
               S.Remittance_Email AS RemittanceEmail,
               S.Carriage_Notes AS CarriageNotes,
               S.Change_Of_Name AS ChangeOfName,
           
               -- Parent
               S.Parent_Name AS ParentName,
               S.Parent_Address_Line1 AS ParentAddressLine1,
               S.Parent_Address_Line2 AS ParentAddressLine2,
               S.Parent_Address_Line3 AS ParentAddressLine3,
               S.Parent_Address_Line4 AS ParentAddressLine4,
               S.Parent_Address_Line5 AS ParentAddressLine5,
               S.Parent_Post_Code AS ParentPostCode,
               S.Parent_Telephone_No AS ParentTelephoneNo,
               S.Parent_Facsimile_No AS ParentFacsimileNo,
           
               -- Carriage
               CAR.SupplierCode AS CarriageSupplierCode,
               CAR.CarriageTypeId AS CarriageTypeId,
               CAR.CarriageBaseId AS CarriageBaseId,
               CAR.CarriageConsignmentId AS CarriageConsignmentId,
               CAR.MinOrderCharge AS MinOrderCharge,
               CAR.OrderValue AS OrderValue,
               CAR.OrderCharge AS OrderCharge,
               CAR.ForYear AS CarriageForYear,
               CAR.FreightIn AS FreightIn,
               CAR.FreightOut AS FreightOut,
               CAR.NorthernIreland AS NorthernIreland,
               CAR.IntoStockFOC AS IntoStockFoc,
               CAR.Offloading_Charge AS OffloadingCharge,
               CAR.General_Comments AS GeneralComments,
           
               -- Current currency
               CC.Currency AS CurrentCurrencyCode,
               CC.Currency_Rate AS CurrencyRate,
           
               -- Current supplier information
               CSI.Supplier_Code AS CurrentSupplierInformationSupplierCode,
               CSI.ForYear AS CurrentSupplierInformationForYear,
               CSI.Contribution AS Contribution,
               CSI.Order_Confirmation AS OrderConfirmation,
               CSI.Payment_Terms AS PaymentTerms,
               CSI.Rebate_End_Date AS RebateEndDate,
               CSI.Rebate_Start_Date AS RebateStartDate,
               CSI.Rebate_Terms AS RebateTerms,
               CSI.Returns_Policy AS CurrentSupplierInformationReturnsPolicy,
               CSI.Terms_And_Conditions AS TermsAndConditions,
               CSI.Rebate_Start_Month AS RebateStartMonth,
               CSI.Rebate_Start_Day AS RebateStartDay,
               CSI.Page_Nos AS PageNos,
               CSI.No_Of_Products AS NoOfProducts,
               CSI.Settlement_Discount AS SettlementDiscount,
               CSI.Useful_Information AS UsefulInformation,
               CSI.Additional_Costs AS AdditionalCosts,
           
               -- Current questionnaire
               CQ.Supplier_Code AS CurrentQuestionnaireSupplierCode,
               CQ.ForYear AS CurrentQuestionnaireForYear,
               CQ.Chase_Letter AS ChaseLetter,
               CQ.Comments AS QuestionnaireComments,
               CQ.Completed_By AS CompletedBy,
               CQ.Completed_By_Job_Title AS CompletedByJobTitle,
               CQ.Completed_Date AS CompletedDate,
               CQ.Date_Sent AS DateSent,
               CQ.Extension AS Extension,
               CQ.Extension_Date AS ExtensionDate,
               CQ.Held_Letter AS HeldLetter,
               CQ.Pricing_Checked AS PricingChecked,
               CQ.Pricing_Held AS PricingHeld,
               CQ.Company_Name AS CompanyName,
               CQ.Address_Line_1 AS QuestionnaireAddressLine1,
               CQ.Address_Line_2 AS QuestionnaireAddressLine2,
               CQ.Address_Line_3 AS QuestionnaireAddressLine3,
               CQ.Address_Line_4 AS QuestionnaireAddressLine4,
               CQ.Address_Line_5 AS QuestionnaireAddressLine5,
               CQ.Post_Code AS QuestionnairePostCode,
               CQ.Telephone_No AS QuestionnaireTelephoneNo,
               CQ.Facsimile_No AS QuestionnaireFacsimileNo,
               CQ.Sales_Contact AS SalesContact,
               CQ.Sales_Contact_Email AS SalesContactEmail,
               CQ.Second_Sales_Contact AS SecondSalesContact,
               CQ.General_Email AS QuestionnaireGeneralEmail,
               CQ.Accounts_Contact AS QuestionnaireAccountsContact,
               CQ.Accounts_Contact_Email AS QuestionnaireAccountsContactEmail,
               CQ.Remittance_Advice_Email AS RemittanceAdviceEmail,
               CQ.Website AS QuestionnaireWebsite,
               CQ.WEEE_Reg AS QuestionnaireWeeeReg,
               CQ.Quality_System AS QuestionnaireQualitySystem,
               CQ.Quality_System_Details AS QuestionnaireQualitySystemDetails,
               CQ.Health_Safety_System AS QuestionnaireHealthSafetySystem,
               CQ.Health_Safety_System_Details AS QuestionnaireHealthSafetySystemDetails,
               CQ.Environment_System AS QuestionnaireEnvironmentSystem,
               CQ.Environment_System_Details AS QuestionnaireEnvironmentSystemDetails,
               CQ.European_Legislation AS EuropeanLegislation,
               CQ.Returns_Policy AS QuestionnaireReturnsPolicy,
           
               -- Contacts (Primary)
               C.Supplier_Code AS ContactSupplierCode,
               C.Id AS ContactId,
               C.Contact_Name AS ContactName,
               C.Email_Address AS EmailAddress,
               C.Facsimile_No AS ContactFacsimileNo,
               C.Mobile_No AS MobileNo,
               C.Telephone_No AS ContactTelephoneNo,
               C.Contact_Type AS ContactType,
               C.Job_Title AS JobTitle,
               C.First_Name AS FirstName,
           
               -- Supplier quality info
               SQI.Supplier_Code AS SupplierQualityInformationSupplierCode,
               SQI.Approved_By AS ApprovedBy,
               SQI.Approval_Date AS ApprovalDate,
               SQI.Comments_Quality_Of_Product AS CommentsQualityOfProduct,
               SQI.Comments_Rating_Quality_Of_Service AS CommentsRatingQualityOfService,
               SQI.Comments_Rating_Delivery_Lead_Times AS CommentsRatingDeliveryLeadTimes,
               SQI.Comments_Rating_Delivery_Timeliness AS CommentsRatingDeliveryTimeliness,
               SQI.Environmental_System AS SupplierQualityInformationEnvironmentalSystem,
               SQI.Health_And_Safety_Considerations AS HealthAndSafetyConsiderations,
               SQI.Health_Safety_System AS SupplierQualityInformationHealthSafetySystem,
               SQI.Quality_System AS SupplierQualityInformationQualitySystem,
               SQI.Rating_Quality_Of_Product AS RatingQualityOfProduct,
               SQI.Rating_Quality_Of_Service AS RatingQualityOfService,
               SQI.Rating_Delivery_Lead_Times AS RatingDeliveryLeadTimes,
               SQI.Rating_Delivery_Timeliness AS RatingDeliveryTimeliness,
               SQI.Sole_Source_Supplier AS SoleSourceSupplier,
               SQI.Last_Audit_Date AS LastAuditDate,
               SQI.Environmental_System_Details AS QualityEnvironmentalSystemDetails,
               SQI.Health_Safety_System_Details AS QualityHealthSafetySystemDetails,
               SQI.Quality_System_Details AS QualitySystemDetails,
               SQI.Product_Type AS ProductType,
           
               -- Supplier notes
               SN.Id AS SupplierNotesId,
               SN.Supplier_Code AS SupplierNotesSupplierCode,
               SN.Notes AS Notes,
               SN.Added_By AS AddedBy,
               SN.Added_Date AS AddedDate,
               
               PMS.ProductManager
           FROM Catalogue.Suppliers AS S
           LEFT JOIN Catalogue.Contacts AS C
               ON C.Supplier_Code = S.Supplier_Code
              AND C.Contact_Type = 'Primary'
           LEFT JOIN Catalogue.CurrentCurrency AS CC
               ON CC.Currency = S.Currency
           LEFT JOIN Catalogue.CurrentQuestionnaire AS CQ
               ON CQ.Supplier_Code = S.Supplier_Code
           LEFT JOIN Catalogue.CurrentSupplierInformation AS CSI
               ON CSI.Supplier_Code = S.Supplier_Code
           LEFT JOIN Catalogue.Carriage AS CAR
               ON CAR.SupplierCode = S.Supplier_Code
           LEFT JOIN Catalogue.CarriageType AS CT
               ON CT.Id = CAR.CarriageTypeId
           LEFT JOIN Catalogue.CarriageBase AS CB
               ON CB.Id = CAR.CarriageBaseId
           LEFT JOIN Catalogue.SupplierQualityInformation AS SQI
               ON SQI.Supplier_Code = S.Supplier_Code
           LEFT JOIN Catalogue.SupplierNotes AS SN
               ON SN.Supplier_Code = S.Supplier_Code
           LEFT JOIN Catalogue.ProductManagersAndSuppliers AS PMS
               ON PMS.Supplier_Code = S.Supplier_Code
           WHERE S.Supplier_Code = @SupplierCode;
           """;

        try
        {
            await using var conn = new SqlConnection(_cs);
            return await conn.QueryFirstOrDefaultAsync<SupplierDetailsRow>(
                new CommandDefinition(sql, new { SupplierCode = supplierCode }, cancellationToken: ct));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IReadOnlyList<SupplierProductRow>> GetSupplierProductsAsync(string supplierCode, int productStatus, CancellationToken ct = default)
    {
        const string sql = """
           SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
           
           SELECT
               Section AS Section,
               Product_Manager AS ProductManager,
               SupplierCode AS SupplierCode,
               Supplier_Product_Code AS SupplierProductCode,
               Object_Type AS ObjectType,
               PageNo AS PageNo,
               OldPageNo AS OldPageNo,
               DesignNo AS DesignNo,
               Overrides AS Overrides,
               Parent_ID AS Parent,
               Base_Code AS BaseCode,
               Long_Description_1 AS Description1,
               Long_Description_2 AS Description2,
               StockStatus AS StockStatus,
               StockControl AS StockControl,
               Green_Flag AS GreenFlag,
               FreeStock AS Stock,
               Run_To_Zero AS RunToZero,
               QtyOnOrder AS OnOrder,
               BackOrdersByCode AS BackOrdersByCode,
               BackOrders AS BackOrders,
               Lead_Time AS LeadTime,
               ListPrice AS ListPrice,
               Discount AS Discount,
               NetCost AS NetCost,
               NetCost2 AS NetCost2,
               NetCost3 AS NetCost3,
               MinQty2 AS MinQty2,
               MinQty3 AS MinQty3,
               Currency_Rate AS CurrencyRate,
               FreightIn AS FreightIn,
               SettlementDiscount AS SettlementDisc,
               StandardCost AS StandardCost,
               CarriageType AS CarriageType,
               FreightOut AS FreightOut,
               [Additional Costs] AS AdditionalCosts,
               Zone_A_Carriage AS ZoneA,
               Zone_B_Carriage AS ZoneB,
               Zone_C_Carriage AS ZoneC,
               Zone_D_Carriage AS ZoneD,
               Zone_E_Carriage AS ZoneE,
               Zone_S_Carriage AS ZoneS,
               CarriageCharge AS CarriageCharge,
               Carriage AS Carriage,
               DespatchMethod AS DespatchMethod,
               TotalCost AS TotalCost,
               Price1 AS Price1,
               Price2 AS Price2,
               Price3 AS Price3,
               Qty2 AS PriceBreak2,
               Qty3 AS PriceBreak3,
               Margin AS Margin,
               SOMargin AS SmallMargin
           FROM Catalogue.fn_SupplierProducts(@SupplierCode, @Status)
           ORDER BY PageNo, DesignNo;
           """;

        await using var conn = new SqlConnection(_cs);
        var rows = await conn.QueryAsync<SupplierProductRow>(new CommandDefinition(sql,
            new { SupplierCode = supplierCode, Status = productStatus }, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<SupplierProductSalesRow>> GetSupplierProductSalesAsync(string supplierCode, int status, CancellationToken ct = default)
    {
        const string sql = """
           SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
           SELECT
               Section,
               Product_Manager AS ProductManager,
               SupplierCode,
               Supplier_Product_Code AS SupplierProductCode,
               Heading,
               PageNo,
               DesignNo,
               Parent_ID AS ParentId,
               Object_Type AS ObjectType,
               Long_Description_1 AS LongDescription1,
               Long_Description_2 AS LongDescription2,
           
               OrdersTwoYear, UnitsTwoYear, CostsTwoYear, SalesTwoYear, ProfitTwoYear,
               OrdersLastYear, UnitsLastYear, CostsLastYear, SalesLastYear, ProfitLastYear,
               OrdersThisYear, UnitsThisYear, CostsThisYear, SalesThisYear, ProfitThisYear,
               OrdersRolling, UnitsRolling, CostsRolling, SalesRolling, ProfitRolling
           FROM Catalogue.fn_SupplierProductSales(@SupplierCode, @Status) ORDER BY DesignNo;  
           """;

        await using var conn = new SqlConnection(_cs);
        var rows = await conn.QueryAsync<SupplierProductSalesRow>(
            new CommandDefinition(sql, new { SupplierCode = supplierCode, Status = status }));
        return rows.AsList();
    }

    public async Task<ProductOverviewRow?> GetProductOverviewAsync(string code, CancellationToken ct = default)
    {
        const string sql = """
                           SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                           SELECT
                                *
                           FROM Catalogue.fn_ProductOverview(@Code);
                           """;

        await using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<ProductOverviewRow>(new CommandDefinition(sql, new { Code = code },
            cancellationToken: ct));
    }
}