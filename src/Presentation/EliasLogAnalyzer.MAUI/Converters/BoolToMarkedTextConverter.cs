using System.Globalization;

namespace EliasLogAnalyzer.MAUI.Converters;

public class BoolToMarkedTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isMarked)
        {
            return isMarked ? "Unmark" : "Mark";
        }

        return "Mark";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}