using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Dolly.Desktop.ViewModels.Tabs;

public sealed partial class CarriageDetailsViewModel : ObservableObject
{
    public ObservableCollection<string> CarriageIsOptions { get; } = ["Stocked", "Free Carriage", "Chargeable"];
    public ObservableCollection<string> YesNoOptions { get; } = ["Yes", "No"];
    public ObservableCollection<int> Years { get; } = [2024, 2025, 2026, 2027];

    [ObservableProperty] private string? _supplier;
    [ObservableProperty] private string? _carriageIs;
    [ObservableProperty] private string? _carriageType;
    [ObservableProperty] private string? _freightInPercentage = "0.00%";
    [ObservableProperty] private string? _freightOutPercentage = "0.00%";
    [ObservableProperty] private string? _minimumOrderCharge = "No";

    [ObservableProperty] private string? _orderValue;
    [ObservableProperty] private string? _orderCharge;

    [ObservableProperty] private string? _zoneDefinitionA;
    [ObservableProperty] private string? _zoneDefinitionB;
    [ObservableProperty] private string? _zoneDefinitionC;
    [ObservableProperty] private string? _zoneDefinitionD;
    [ObservableProperty] private string? _zoneDefinitionE;

    [ObservableProperty] private int _selectedYear = 2026;
    [ObservableProperty] private string? _northernIrelandDelivery = "Northern Ireland";
    [ObservableProperty] private string? _intoStockFoc;
    [ObservableProperty] private string? _carriageNotes;

    [ObservableProperty] private string? _carriageRateA1;
    [ObservableProperty] private string? _carriageRateA2;
    [ObservableProperty] private string? _carriageRateA3;
    [ObservableProperty] private string? _carriageRateA4;
    [ObservableProperty] private string? _carriageRateA5;
    [ObservableProperty] private string? _carriageRateA6;
    [ObservableProperty] private string? _carriageRateA7;
    [ObservableProperty] private string? _carriageRateA8;
    [ObservableProperty] private string? _carriageRateA9;
    [ObservableProperty] private string? _carriageRateA10;

    [ObservableProperty] private string? _carriageRateB1;
    [ObservableProperty] private string? _carriageRateB2;
    [ObservableProperty] private string? _carriageRateB3;
    [ObservableProperty] private string? _carriageRateB4;
    [ObservableProperty] private string? _carriageRateB5;
    [ObservableProperty] private string? _carriageRateB6;
    [ObservableProperty] private string? _carriageRateB7;
    [ObservableProperty] private string? _carriageRateB8;
    [ObservableProperty] private string? _carriageRateB9;
    [ObservableProperty] private string? _carriageRateB10;

    [ObservableProperty] private string? _carriageRateC1;
    [ObservableProperty] private string? _carriageRateC2;
    [ObservableProperty] private string? _carriageRateC3;
    [ObservableProperty] private string? _carriageRateC4;
    [ObservableProperty] private string? _carriageRateC5;
    [ObservableProperty] private string? _carriageRateC6;
    [ObservableProperty] private string? _carriageRateC7;
    [ObservableProperty] private string? _carriageRateC8;
    [ObservableProperty] private string? _carriageRateC9;
    [ObservableProperty] private string? _carriageRateC10;

    [ObservableProperty] private string? _carriageRateD1;
    [ObservableProperty] private string? _carriageRateD2;
    [ObservableProperty] private string? _carriageRateD3;
    [ObservableProperty] private string? _carriageRateD4;
    [ObservableProperty] private string? _carriageRateD5;
    [ObservableProperty] private string? _carriageRateD6;
    [ObservableProperty] private string? _carriageRateD7;
    [ObservableProperty] private string? _carriageRateD8;
    [ObservableProperty] private string? _carriageRateD9;
    [ObservableProperty] private string? _carriageRateD10;
    
    [ObservableProperty] private string? _carriageRateE1;
    [ObservableProperty] private string? _carriageRateE2;
    [ObservableProperty] private string? _carriageRateE3;
    [ObservableProperty] private string? _carriageRateE4;
    [ObservableProperty] private string? _carriageRateE5;
    [ObservableProperty] private string? _carriageRateE6;
    [ObservableProperty] private string? _carriageRateE7;
    [ObservableProperty] private string? _carriageRateE8;
    [ObservableProperty] private string? _carriageRateE9;
    [ObservableProperty] private string? _carriageRateE10;
    [ObservableProperty] private string? _stockedRate;

    [RelayCommand]
    private Task EditAsync()
    {
        // Add carriage edit/save logic here later.
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OpenPricingQuestionnaireAsync()
    {
        // Connect this to the pricing questionnaire window/service later.
        return Task.CompletedTask;
    }
}