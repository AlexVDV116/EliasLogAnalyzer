using System.Globalization;
using Microsoft.Maui.Controls;

namespace EliasLogAnalyzer.MAUI.Converters
{
    public class TimeDeltaToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || value is not int)
            {
                return string.Empty;
            }

            int timeDelta = (int)value;

            if (timeDelta == 0)
            {
                return "0 ms";
            }

            bool isPositive = timeDelta >= 0;

            timeDelta = Math.Abs(timeDelta);

            if (timeDelta <= 10000)
            {
                return $"{(isPositive ? "+" : "-")}{timeDelta} ms";
            }
            else if (timeDelta < 60000) // Less than 60 seconds
            {
                double seconds = timeDelta / 1000.0;
                return $"{(isPositive ? "+" : "-")}{FormatNumber(seconds)} s";
            }
            else if (timeDelta < 3600000) // Less than 60 minutes
            {
                double minutes = timeDelta / 60000.0;
                return $"{(isPositive ? "+" : "-")}{FormatNumber(minutes)} m";
            }
            else // 60 minutes or more
            {
                double hours = timeDelta / 3600000.0;
                return $"{(isPositive ? "+" : "-")}{FormatNumber(hours)} h";
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string FormatNumber(double number)
        {
            if (number % 1 == 0)
            {
                return ((int)number).ToString(CultureInfo.InvariantCulture);
            }
            return number.ToString("F1", CultureInfo.InvariantCulture);
        }
    }
}
