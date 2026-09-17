using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Dolly.Desktop.ViewModels.Tabs;

public sealed partial class AdditionalInfoViewModel : ObservableObject
{
    public ObservableCollection<string> StatusOptions { get; } = ["NEW", "ACTIVE", "DISCONTINUED"];
    public ObservableCollection<string> YesNoOptions { get; } = ["Yes", "No"];
    public ObservableCollection<string> MonthOptions { get; } = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    public ObservableCollection<int> DayOptions { get; } =
        Enumerable.Range(1, 31).ToList() is var days
            ? new ObservableCollection<int>(days)
            : [];

    public ObservableCollection<string> CurrencyOptions { get; } = ["GBP", "USD", "EUR"];

    [ObservableProperty] private string? _previousNames;
    [ObservableProperty] private string? _currentStatus = "NEW";
    [ObservableProperty] private string? _yearLastUsed;
    [ObservableProperty] private string? _discontinuedReason;
    [ObservableProperty] private string? _orderConfirmation;
    [ObservableProperty] private string? _euCountry = "No";
    [ObservableProperty] private string? _hasFile = "Yes";
    [ObservableProperty] private string? _imported = "No";
    [ObservableProperty] private string? _weeeReg;

    [ObservableProperty] private string? _supplierMessage;
    [ObservableProperty] private string? _usefulInformation;

    [ObservableProperty] private string? _paymentTerms;
    [ObservableProperty] private string? _settlementDiscount = "0.00%";
    [ObservableProperty] private string? _rebateTerms;
    [ObservableProperty] private string? _rebateStartMonth;
    [ObservableProperty] private string? _rebateStartDay;
    [ObservableProperty] private string? _returnsPolicy;

    [ObservableProperty] private string? _additionalCosts;
    [ObservableProperty] private string? _contributionAmount;
    [ObservableProperty] private string? _currency = "GBP";
    [ObservableProperty] private string? _currencyRate = "1.0000";
    [ObservableProperty] private string? _pricingStartMonth = "Feb";

    [RelayCommand]
    private Task OpenCarriageCalculatorAsync() => Task.CompletedTask;

    [RelayCommand]
    private Task OpenPartCodeLookupAsync() => Task.CompletedTask;

    [RelayCommand]
    private Task CalculateRoiSellingAsync() => Task.CompletedTask;

    [RelayCommand]
    private Task EditAdditionalCostsAsync() => Task.CompletedTask;
}