using dotNetFractal.Logic;
using System;
using System.Globalization;
using System.Windows.Data;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// Converts between FractalDecimal or FractalDouble and string for TextBox bindings.
    /// </summary>
    public class FractalUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return string.Empty;

            if (value is FractalDecimal fractalDecimal)
            {
                decimal decimalValue = (decimal)fractalDecimal;
                return decimalValue.ToString(culture);
            }

            if (value is FractalDouble fractalDouble)
            {
                double doubleValue = (double)fractalDouble;
                return doubleValue.ToString(culture);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string stringValue || string.IsNullOrWhiteSpace(stringValue))
                return Binding.DoNothing;

            // Determine if we're converting to FractalDecimal or FractalDouble
            if (targetType == typeof(FractalDecimal))
            {
                if (decimal.TryParse(stringValue, NumberStyles.Float, culture, out decimal decimalValue))
                {
                    return (FractalDecimal)decimalValue;
                }
            }
            else if (targetType == typeof(FractalDouble))
            {
                if (double.TryParse(stringValue, NumberStyles.Float, culture, out double doubleValue))
                {
                    return (FractalDouble)doubleValue;
                }
            }

            return Binding.DoNothing;
        }
    }
}
