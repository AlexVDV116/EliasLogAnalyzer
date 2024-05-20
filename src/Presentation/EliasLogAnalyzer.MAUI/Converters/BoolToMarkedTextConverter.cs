using System.Globalization;

namespace EliasLogAnalyzer.MAUI.Converters;

public class BoolToMarkedTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isMarked)
        {
            return isMarked ? "\u2b50 Unmark" : "\u2b50 Mark";
        }

        return "\u2b50 Mark";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}