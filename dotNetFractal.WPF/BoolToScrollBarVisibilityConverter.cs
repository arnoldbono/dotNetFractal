using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// Converts a boolean to ScrollBarVisibility.
    /// True = Disabled (no scrollbars when stretching)
    /// False = Auto (show scrollbars when needed)
    /// </summary>
    public class BoolToScrollBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool stretch)
            {
                return stretch ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
            }
            return ScrollBarVisibility.Auto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ScrollBarVisibility visibility)
            {
                return visibility == ScrollBarVisibility.Disabled;
            }
            return false;
        }
    }
}
