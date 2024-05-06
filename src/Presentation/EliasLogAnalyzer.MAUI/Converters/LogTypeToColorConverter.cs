using System.Globalization;

namespace EliasLogAnalyzer.MAUI.Converters
{
    public class LogTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string logType = value as string;

            switch (logType)
            {
                case "Error":
                    return Colors.Red;
                case "Fatal":
                    return Colors.Red;
                case "Warning":
                    return Colors.Orange;
                case "Warn":
                    return Colors.Orange;
                case "Information":
                    return Colors.Yellow;
                case "Info":
                    return Colors.Yellow;
                case "Debug":
                    return Colors.Black;
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