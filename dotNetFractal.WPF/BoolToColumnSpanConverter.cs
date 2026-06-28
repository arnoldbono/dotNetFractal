using System;
using System.Globalization;
using System.Windows.Data;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// Converts a boolean to a column span value.
    /// True (properties panel is not collapsed) returns 3 (spans all columns), False returns 1 (single column).
    /// </summary>
    public class BoolToColumnSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? 1 : 3;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
