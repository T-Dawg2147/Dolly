using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels.Tabs;

public sealed partial class SupplierDetailsViewModel : ObservableObject
{
    private readonly ISupplierQueriesService _queries;
    private readonly ISupplierDetailsCache _cache;
    private readonly SupplierContextState _context;
    
    public SupplierDetailsViewModel(ISupplierQueriesService queries, ISupplierDetailsCache cache, SupplierContextState context)
    {
        _queries = queries;
        _cache = cache;
        _context = context;

        SendQuestionnaireOptions.Add("No");
        SendQuestionnaireOptions.Add("Question");
        ExtensionOptions.Add("No");
        ExtensionOptions.Add("Extension");
        HeldOptions.Add("No");
        HeldOptions.Add("Held Letter");

        SendQuestionnaire = "No";
        GrantExtension = "No";
        HeldLetter = "No";
        ShowWithdrawn = true;

        _ = LoadSuppliersAsync();

        _context.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(SupplierContextState.SelectedSupplierCode))
                await LoadSupplierAsync(_context.SelectedSupplierCode);
        };
    }
    
    #region Private Properties
    [ObservableProperty] private SupplierSummary? _selectedSupplier;
    [ObservableProperty] private string _supplierSearchText = "";
    
     // ---------- Supplier basics ----------
    [ObservableProperty] private string? _supplierCode;
    [ObservableProperty] private string? _supplierName;
    [ObservableProperty] private string? _fullName;

    // ---------- Supplier address ----------
    [ObservableProperty] private string? _addressLine1;
    [ObservableProperty] private string? _addressLine2;
    [ObservableProperty] private string? _addressLine3;
    [ObservableProperty] private string? _addressLine4;
    [ObservableProperty] private string? _addressLine5;
    [ObservableProperty] private string? _country;
    [ObservableProperty] private string? _postCode;
    [ObservableProperty] private string? _telephoneNo;
    [ObservableProperty] private string? _faxNo;
    [ObservableProperty] private string? _buyer;
    [ObservableProperty] private string? _productManager;

    // ---------- Contact ----------
    [ObservableProperty] private string? _contactName;
    [ObservableProperty] private string? _contactEmail;
    [ObservableProperty] private string? _contactTelNos;

    // ---------- Parent address ----------
    [ObservableProperty] private string? _parentAddressLine1;
    [ObservableProperty] private string? _parentAddressLine2;
    [ObservableProperty] private string? _parentAddressLine3;
    [ObservableProperty] private string? _parentAddressLine4;
    [ObservableProperty] private string? _parentAddressLine5;
    [ObservableProperty] private string? _parentPostCode;
    [ObservableProperty] private string? _parentTelephoneNo;
    [ObservableProperty] private string? _website;

    // ---------- View options ----------
    [ObservableProperty] private bool _backOrdersByProduct;
    [ObservableProperty] private bool _backOrdersByParent;
    [ObservableProperty] private bool _reportingSupplier;
    [ObservableProperty] private bool _hideWithdrawn;
    [ObservableProperty] private bool _showWithdrawn = true;
    [ObservableProperty] private bool _containsSupplier;

    // ---------- Pricing Questionnaire ----------
    [ObservableProperty] private string? _sendQuestionnaire;
    [ObservableProperty] private string? _dateSent;
    [ObservableProperty] private string? _dateCompleted;
    [ObservableProperty] private string? _grantExtension;
    [ObservableProperty] private string? _extensionDate;
    [ObservableProperty] private string? _chaseLetterDate;
    [ObservableProperty] private string? _heldLetter;
    [ObservableProperty] private string? _pricingComments;
    [ObservableProperty] private bool _showDelSuppliers; // TODO: I have never seen this checkbox in my LIFE. gotta look int functionality
    #endregion

    public ObservableCollection<string> SendQuestionnaireOptions { get; } = ["No", "Question"]; // TODO want to look through all collections, make sure the values are correct.
    public ObservableCollection<string> ExtensionOptions { get; } = ["No", "Extension"]; // TODO
    public ObservableCollection<string> HeldOptions { get; } = ["No", "Held Letter"]; // TODO
    public ObservableCollection<string> Conutries { get; } = [];
    public ObservableCollection<SupplierSummary> Suppliers { get; } = [];

    partial void OnReportingSupplierChanged(bool value) => _context.ReportingSupplier = value;
    partial void OnContainsSupplierChanged(bool value) => _context.ContainsSupplier = value;
    partial void OnShowWithdrawnChanged(bool value) => _context.ProductStatus = value ? 1 : 0;
    partial void OnSelectedSupplierChanged(SupplierSummary? value)
    {
        _context.SelectedSupplierCode = value?.SupplierCode;
        _ = LoadSupplierAsync(_context.SelectedSupplierCode);
    }
    
    [RelayCommand]
    private async Task LoadSuppliersAsync()
    {
        var results = await _queries.SearchSuppliersAsync(SupplierSearchText);
        Suppliers.Clear();
        foreach (var s in results) Suppliers.Add(s);
    }
    
    [RelayCommand]
    public async Task LoadSupplierAsync(string? supplierCodeArg)
    {
        _context.SelectedSupplierCode = supplierCodeArg;
        
        if (string.IsNullOrWhiteSpace(supplierCodeArg))
        {
            _context.SelectedSupplierDetails = null;
            ClearSupplierFields();
            return;
        }

        // Checks to see if supplier is in supplier cache
        SupplierDetailsRow? details;
        if (_cache.TryGet(supplierCodeArg, out var cached))
        {
            details = cached; // Supplier IS in cache, pass through
        }
        else
        {
            // Supplier is NOT in cache, get from database
            details = await _queries.GetSupplierDetailsAsync(supplierCodeArg);
            if (details is not null)
                _cache.Set(supplierCodeArg, details);
        }

        _context.SelectedSupplierDetails = details;

        if (details is null)
        {
            ClearSupplierFields();
            return;
        }

        // Map row values from memory to UI
        MapSupplierFromRow(details);
    }
    
    // TODO: The below code are all stub methods; they do nothing, just fill the hole until code is added
    #region To look at
    [RelayCommand]
    private Task ChangeNameAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task EditSupplierAddressAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task DataLoadAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task GetNewSupplierAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task EditContactAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task CloseAsync() // TODO: pretty sure i can place the close command in a higher VM
    {
        return Task.CompletedTask;
    }
    #endregion

    private void MapSupplierFromRow(SupplierDetailsRow details)
    {
        SupplierCode = details.SupplierCode;
        SupplierName = details.SupplierName ?? "";
        FullName = details.FullName ?? "";

        AddressLine1 = details.AddressLine1 ?? "";
        AddressLine2 = details.AddressLine2 ?? "";
        AddressLine3 = details.AddressLine3 ?? "";
        AddressLine4 = details.AddressLine4 ?? "";
        AddressLine5 = details.AddressLine5 ?? "";
        Country = details.Country ?? "";
        PostCode = details.PostCode ?? "";
        TelephoneNo = details.TelephoneNo ?? "";
        FaxNo = details.FacsimileNo ?? "";
        Website = details.Website ?? "";
        Buyer = details.Buyer ?? "";
        ProductManager = details.ProductManager ?? "";

        ContactName = details.ContactName ?? "";
        ContactEmail = details.EmailAddress ?? "";
        ContactTelNos = $"{details.ContactTelephoneNo ?? ""} {details.MobileNo ?? ""}".Trim();

        ParentAddressLine1 = details.ParentAddressLine1 ?? "";
        ParentAddressLine2 = details.ParentAddressLine2 ?? "";
        ParentAddressLine3 = details.ParentAddressLine3 ?? "";
        ParentAddressLine4 = details.ParentAddressLine4 ?? "";
        ParentAddressLine5 = details.ParentAddressLine5 ?? "";
        ParentPostCode = details.ParentPostCode ?? "";
        ParentTelephoneNo = details.ParentTelephoneNo ?? "";

        SendQuestionnaire = details.SendQuestionnaire ?? "";
        DateSent = details.DateSent.ToString(); // DateOnly
        DateCompleted = details.DateCompleted.ToString(); // DateOnly
        GrantExtension = details.GrantExtension; // Yes/No
        ExtensionDate = details.ExtensionDate.ToString(); // DateOnly
        ChaseLetterDate = details.ChaseLetterDate; // DateOnly
        HeldLetter = details.HeldLetter; // Yes/No
        PricingComments = details.PricingComments;
        

        // questionnaire / options (adjust once exact DB fields confirmed)
        PricingComments = details.PricingComments ?? "";
        ReportingSupplier = _context.ReportingSupplier;
        ContainsSupplier = _context.ContainsSupplier;
    }
    
    private void ClearSupplierFields()
    {
        SupplierCode = SupplierName = FullName = "";
        AddressLine1 = AddressLine2 = AddressLine3 = AddressLine4 = AddressLine5 = "";
        Country = PostCode = TelephoneNo = FaxNo = Website = Buyer = ProductManager = "";
        ContactName = ContactEmail = ContactTelNos = "";
        ParentAddressLine1 = ParentAddressLine2 = ParentAddressLine3 = ParentAddressLine4 = ParentAddressLine5 = "";
        ParentPostCode = ParentTelephoneNo = "";
        PricingComments = "";
    }
}