using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Dolly.Desktop.Converters;

public sealed class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b)
        {
            return new SolidColorBrush(Color.FromArgb(200, 255, 150, 150));
        }

        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}