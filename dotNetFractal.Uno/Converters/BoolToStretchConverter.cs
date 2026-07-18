using System;

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace dotNetFractal.Uno.Converters
{
    /// <summary>
    /// Converts a boolean to a Stretch value.
    /// True = Uniform (stretches while maintaining aspect ratio)
    /// False = None (no stretching)
    /// </summary>
    public class BoolToStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool stretch)
            {
                return stretch ? Stretch.Uniform : Stretch.None;
            }
            return Stretch.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Stretch stretch)
            {
                return stretch != Stretch.None;
            }
            return false;
        }
    }
}
