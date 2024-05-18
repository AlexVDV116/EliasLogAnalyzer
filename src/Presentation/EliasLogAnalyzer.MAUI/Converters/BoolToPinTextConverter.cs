using System.Diagnostics;
using System.Globalization;

namespace EliasLogAnalyzer.MAUI.Converters;

public class BoolToPinTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isPinned)
        {
            return isPinned ? "\U0001F4CC Unpin" : "\U0001F4CC Pin";
        }

        return "\U0001F4CC Pin";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}