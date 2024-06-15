using System.Globalization;

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

            var timeDelta = (int)value;

            if (timeDelta == 0)
            {
                return "0 ms";
            }

            var isPositive = timeDelta >= 0;

            timeDelta = Math.Abs(timeDelta);

            switch (timeDelta)
            {
                case <= 10000:
                    return $"{(isPositive ? "+" : "-")}{timeDelta} ms";
                // Less than 60 seconds
                case < 60000:
                {
                    var seconds = timeDelta / 1000.0;
                    return $"{(isPositive ? "+" : "-")}{FormatNumber(seconds)} s";
                }
                // Less than 60 minutes
                case < 3600000:
                {
                    var minutes = timeDelta / 60000.0;
                    return $"{(isPositive ? "+" : "-")}{FormatNumber(minutes)} m";
                }
                // 60 minutes or more
                default:
                {
                    var hours = timeDelta / 3600000.0;
                    return $"{(isPositive ? "+" : "-")}{FormatNumber(hours)} h";
                }
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string FormatNumber(double number)
        {
            if (number % 1 == 0)
            {
                return ((int)number).ToString(CultureInfo.InvariantCulture);
            }
            return number.ToString("F1", CultureInfo.InvariantCulture);
        }
    }
}
