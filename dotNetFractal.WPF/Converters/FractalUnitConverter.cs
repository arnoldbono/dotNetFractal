using System;
using System.Globalization;
using System.Windows.Data;

namespace dotNetFractal.WPF.Converters;

/// <summary>
/// Converts between decimal or double and string for TextBox bindings.
/// </summary>
public class FractalUnitConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue.ToString(culture);
        }

        if (value is double doubleValue)
        {
            return doubleValue.ToString(culture);
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string stringValue || string.IsNullOrWhiteSpace(stringValue))
            return Binding.DoNothing;

        // Determine if we're converting to decimal or double
        if (targetType == typeof(decimal))
        {
            if (decimal.TryParse(stringValue, NumberStyles.Float, culture, out decimal decimalValue))
            {
                return decimalValue;
            }
        }
        else if (targetType == typeof(double))
        {
            if (double.TryParse(stringValue, NumberStyles.Float, culture, out double doubleValue))
            {
                return doubleValue;
            }
        }

        return Binding.DoNothing;
    }
}
