using System;
using System.Globalization;
using System.Windows.Data;

namespace dotNetFractal.WPF;

public class EnumBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(targetType, nameof(targetType));
        if (!targetType.IsInstanceOfType(parameter))
        {
            throw new ArgumentException($"Parameter must be of type {targetType}", nameof(parameter));
        }
        return (bool)value ? parameter : default!;
    }
}
