using System.Globalization;

namespace EliasLogAnalyzer.MAUI.Converters;

public class FileSizeFormatConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not long bytes) return "0 Bytes";
        const long kilobyte = 1024;
        const long megabyte = 1024 * kilobyte;

        switch (bytes)
        {
            case >= megabyte:
                return $"{Math.Round(bytes / (double)megabyte, 2)} MB";
            case >= kilobyte:
                return $"{Math.Round(bytes / (double)kilobyte)} kB";
            default:
                return $"{bytes} B";
        }
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}