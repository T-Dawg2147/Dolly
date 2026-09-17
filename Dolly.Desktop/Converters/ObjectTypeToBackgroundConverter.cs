using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Dolly.Desktop.Converters;

public sealed class ObjectTypeToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string objectType && !string.IsNullOrWhiteSpace(objectType))
        {
            if (objectType.Contains("Withdrawn", StringComparison.OrdinalIgnoreCase))
                return new SolidColorBrush(Color.FromArgb(150, 255, 107, 107));
        }

        return new SolidColorBrush(Colors.Transparent);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}