using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace dotNetFractal.Uno.Converters
{
    /// <summary>
    /// Converts between decimal or double and string for TextBox bindings.
    /// </summary>
    public class FractalUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return string.Empty;

            if (value is decimal decimalValue)
            {
                return decimalValue.ToString(CultureInfo.InvariantCulture);
            }

            if (value is double doubleValue)
            {
                return doubleValue.ToString(CultureInfo.InvariantCulture);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not string stringValue || string.IsNullOrWhiteSpace(stringValue))
                return null;

            // Determine if we're converting to decimal or double
            if (targetType == typeof(decimal))
            {
                if (decimal.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalValue))
                {
                    return decimalValue;
                }
            }
            else if (targetType == typeof(double))
            {
                if (double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
                {
                    return doubleValue;
                }
            }

            return null;
        }
    }
}
