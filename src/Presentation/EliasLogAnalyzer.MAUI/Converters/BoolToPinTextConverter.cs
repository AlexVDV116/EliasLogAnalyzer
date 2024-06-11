using System.Globalization;
using Microsoft.Maui.Controls;

namespace EliasLogAnalyzer.MAUI.Converters;

public class BoolToPinTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isPinned)
        {
            return isPinned ? "Unpin" : "Pin";
        }

        return "Pin";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}