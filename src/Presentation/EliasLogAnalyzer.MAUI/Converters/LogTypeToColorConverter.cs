using System.Globalization;
using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Converters
{
    public class LogTypeToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LogType logType)
            {
                return logType switch
                {
                    LogType.Error => Color.FromRgba(255, 0, 0, 0.1),        // Red with 20% opacity
                    LogType.Warning => Color.FromRgba(255, 165, 0, 0.1),   // Orange with 20% opacity
                    LogType.Information => Color.FromRgba(0, 255, 0, 0.1), // Green with 20% opacity
                    LogType.Debug => Color.FromRgba(0, 0, 0, 0.1),         // Black with 20% opacity
                    LogType.None => Color.FromRgba(128, 128, 128, 0.1),    // Gray with 20% opacity
                    _ => Color.FromRgba(128, 128, 128, 0.1)                // Default to gray with 20% opacity
                };
            }

            return Color.FromRgba(128, 128, 128, 0.2); // Default Gray with 20% opacity
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}