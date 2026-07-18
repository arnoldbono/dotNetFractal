using System;

using Microsoft.UI.Xaml.Data;

namespace dotNetFractal.Uno.Converters
{
    /// <summary>
    /// Converts a boolean to collapse/expand symbols for the properties panel.
    /// True (expanded) returns "▶", False (collapsed) returns "◀"
    /// </summary>
    public class BoolToCollapseSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? "▶" : "◀";
            }
            return "▶";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
