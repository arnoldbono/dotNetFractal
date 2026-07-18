using System;
using Microsoft.UI.Xaml.Data;

namespace dotNetFractal.Uno.Converters
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? parameter : null;
        }
    }
}
