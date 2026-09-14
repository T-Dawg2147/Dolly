using System.Globalization;
using System.Windows.Data;

namespace Dolly.Desktop.Converters;

public sealed class BoolToInvalidTagConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b ? "Invalid" : "Valid"; 

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}