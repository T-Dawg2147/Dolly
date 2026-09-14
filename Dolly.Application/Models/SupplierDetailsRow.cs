namespace Dolly.Application.Models;

public sealed class SupplierDetailsRow
{
    // Suppliers
    public string SupplierCode { get; set; } = "";
    public string? SupplierName { get; set; }
    public string? FullName { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? AddressLine3 { get; set; }
    public string? AddressLine4 { get; set; }
    public string? AddressLine5 { get; set; }
    public string? Country { get; set; }
    public string? PostCode { get; set; }
    public string? TelephoneNo { get; set; }
    public string? FacsimileNo { get; set; }
    public string? Website { get; set; }
    public string? GeneralEmail { get; set; }
    public string? Buyer { get; set; }
    public string? Currency { get; set; }
    public string? CurrentStatus { get; set; }
    public string? DiscontinuedReason { get; set; }
    public string? EuCountry { get; set; }
    public string? HasFile { get; set; }
    public string? Imported { get; set; }
    public string? ProductsConformLegislation { get; set; }
    public string? SendQuestionnaire { get; set; }
    public string? SupplierMessage { get; set; }
    public string? WeeeReg { get; set; }
    public string? PricingStartMonth { get; set; }
    public int? LastUsed { get; set; } // DateTime Year
    public string? QualitySystem { get; set; }
    public string? EnvironmentalSystem { get; set; }
    public string? HealthSafetySystem { get; set; }
    public string? AccountsContact { get; set; }
    public string? AccountsContactEmail { get; set; }
    public string? RemittanceEmail { get; set; }
    public string? CarriageNotes { get; set; }
    public string? ChangeOfName { get; set; }

    // Parent details
    public string? ParentName { get; set; }
    public string? ParentAddressLine1 { get; set; }
    public string? ParentAddressLine2 { get; set; }
    public string? ParentAddressLine3 { get; set; }
    public string? ParentAddressLine4 { get; set; }
    public string? ParentAddressLine5 { get; set; }
    public string? ParentPostCode { get; set; }
    public string? ParentTelephoneNo { get; set; }
    public string? ParentFacsimileNo { get; set; }

    // Carriage
    public string? CarriageSupplierCode { get; set; }
    public int? CarriageTypeId { get; set; }
    public int? CarriageBaseId { get; set; }
    public int? CarriageConsignmentId { get; set; }
    public string? MinOrderCharge { get; set; }
    public decimal? OrderValue { get; set; }
    public decimal? OrderCharge { get; set; }
    public int? CarriageForYear { get; set; }
    public decimal? FreightIn { get; set; }
    public decimal? FreightOut { get; set; }
    public string? NorthernIreland { get; set; }
    public string? IntoStockFoc { get; set; }
    public string? OffloadingCharge { get; set; }
    public string? GeneralComments { get; set; }

    // Currency
    public string? CurrentCurrencyCode { get; set; }
    public decimal? CurrencyRate { get; set; }

    // Current supplier info
    public string? CurrentSupplierInformationSupplierCode { get; set; }
    public int? CurrentSupplierInformationForYear { get; set; }
    public decimal? Contribution { get; set; }
    public string? OrderConfirmation { get; set; }
    public string? PaymentTerms { get; set; }
    public DateTime? RebateEndDate { get; set; }
    public DateTime? RebateStartDate { get; set; }
    public string? RebateTerms { get; set; }
    public string? CurrentSupplierInformationReturnsPolicy { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? RebateStartMonth { get; set; }
    public int? RebateStartDay { get; set; }
    public string? PageNos { get; set; }
    public int? NoOfProducts { get; set; }
    public decimal? SettlementDiscount { get; set; }
    public string? UsefulInformation { get; set; }
    public decimal? AdditionalCosts { get; set; }

    // Current questionnaire
    public string? CurrentQuestionnaireSupplierCode { get; set; }
    public int? CurrentQuestionnaireForYear { get; set; }
    public string? ChaseLetterDate { get; set; }
    public string? PricingComments { get; set; }
    public string? CompletedBy { get; set; }
    public string? CompletedByJobTitle { get; set; }
    public DateTime? DateCompleted { get; set; }
    public DateTime? DateSent { get; set; }
    public string? GrantExtension { get; set; }
    public DateTime? ExtensionDate { get; set; }
    public string? HeldLetter { get; set; }
    public string? PricingChecked { get; set; }
    public string? PricingHeld { get; set; }
    public string? CompanyName { get; set; }
    public string? QuestionnaireAddressLine1 { get; set; }
    public string? QuestionnaireAddressLine2 { get; set; }
    public string? QuestionnaireAddressLine3 { get; set; }
    public string? QuestionnaireAddressLine4 { get; set; }
    public string? QuestionnaireAddressLine5 { get; set; }
    public string? QuestionnairePostCode { get; set; }
    public string? QuestionnaireTelephoneNo { get; set; }
    public string? QuestionnaireFacsimileNo { get; set; }
    public string? SalesContact { get; set; }
    public string? SalesContactEmail { get; set; }
    public string? SecondSalesContact { get; set; }
    public string? QuestionnaireGeneralEmail { get; set; }
    public string? QuestionnaireAccountsContact { get; set; }
    public string? QuestionnaireAccountsContactEmail { get; set; }
    public string? RemittanceAdviceEmail { get; set; }
    public string? QuestionnaireWebsite { get; set; }
    public string? QuestionnaireWeeeReg { get; set; }
    public string? QuestionnaireQualitySystem { get; set; }
    public string? QuestionnaireQualitySystemDetails { get; set; }
    public string? QuestionnaireHealthSafetySystem { get; set; }
    public string? QuestionnaireHealthSafetySystemDetails { get; set; }
    public string? QuestionnaireEnvironmentSystem { get; set; }
    public string? QuestionnaireEnvironmentSystemDetails { get; set; }
    public string? EuropeanLegislation { get; set; }
    public string? QuestionnaireReturnsPolicy { get; set; }

    // Contact (Primary)
    public string? ContactSupplierCode { get; set; }
    public int? ContactId { get; set; }
    public string? ContactName { get; set; }
    public string? EmailAddress { get; set; }
    public string? ContactFacsimileNo { get; set; }
    public string? MobileNo { get; set; }
    public string? ContactTelephoneNo { get; set; }
    public string? ContactType { get; set; }
    public string? JobTitle { get; set; }
    public string? FirstName { get; set; }

    // Quality info
    public string? SupplierQualityInformationSupplierCode { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? CommentsQualityOfProduct { get; set; }
    public string? CommentsRatingQualityOfService { get; set; }
    public string? CommentsRatingDeliveryLeadTimes { get; set; }
    public string? CommentsRatingDeliveryTimeliness { get; set; }
    public string? SupplierQualityInformationEnvironmentalSystem { get; set; }
    public string? HealthAndSafetyConsiderations { get; set; }
    public string? SupplierQualityInformationHealthSafetySystem { get; set; }
    public string? SupplierQualityInformationQualitySystem { get; set; }
    public string? RatingQualityOfProduct { get; set; }
    public string? RatingQualityOfService { get; set; }
    public string? RatingDeliveryLeadTimes { get; set; }
    public string? RatingDeliveryTimeliness { get; set; }
    public string? SoleSourceSupplier { get; set; }
    public DateTime? LastAuditDate { get; set; }
    public string? QualityEnvironmentalSystemDetails { get; set; }
    public string? QualityHealthSafetySystemDetails { get; set; }
    public string? QualitySystemDetails { get; set; }
    public string? ProductType { get; set; }

    // Notes
    public int? SupplierNotesId { get; set; }
    public string? SupplierNotesSupplierCode { get; set; }
    public string? Notes { get; set; }
    public string? AddedBy { get; set; }
    public DateTime? AddedDate { get; set; }

    public string? ProductManager { get; set; }
}