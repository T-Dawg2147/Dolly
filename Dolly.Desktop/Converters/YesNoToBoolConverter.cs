using System.Globalization;
using System.Windows.Data;

namespace Dolly.Desktop.Converters;

public sealed class YesNoToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.ToString()?.Equals("Yes", StringComparison.OrdinalIgnoreCase) == true;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool and true ? "Yes" : "No";
}