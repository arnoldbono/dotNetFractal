using System;
using Microsoft.UI.Xaml.Data;

namespace dotNetFractal.Uno.Converters;

public class EnumBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
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
