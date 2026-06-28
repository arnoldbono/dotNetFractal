using System;
using System.Globalization;
using System.Windows.Data;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// Converts a boolean to collapse/expand symbols for the properties panel.
    /// True (expanded) returns "▶", False (collapsed) returns "◀"
    /// </summary>
    public class BoolToCollapseSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? "▶" : "◀";
            }
            return "◀";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
