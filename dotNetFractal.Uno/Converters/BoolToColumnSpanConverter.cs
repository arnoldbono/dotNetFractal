using System;

using Microsoft.UI.Xaml.Data;

namespace dotNetFractal.Uno.Converters
{
    /// <summary>
    /// Converts a boolean to a column span value.
    /// True (properties panel is not collapsed) returns 3 (spans all columns), False returns 1 (single column).
    /// </summary>
    public class BoolToColumnSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? 1 : 3;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
