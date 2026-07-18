using System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace dotNetFractal.Uno.Converters
{
    /// <summary>
    /// Converts a boolean to Visibility, with inverse logic.
    /// True = Collapsed (hide when condition is true, e.g., full screen)
    /// False = Visible (show when condition is false, e.g., not full screen)
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return false;
        }
    }
}
