using System.Globalization;
using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Converters
{
    public class LogTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var logType = value;

            switch (logType)
            {
                case LogType.Error:
                    return Colors.Red;
                case LogType.Warning:
                    return Colors.Orange;
                case LogType.Information:
                    return Colors.Green;
                case LogType.Debug:
                    return Colors.Black;
                case LogType.None:
                    return Colors.Gray;
                default:
                    return Colors.Gray; // Default color
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}