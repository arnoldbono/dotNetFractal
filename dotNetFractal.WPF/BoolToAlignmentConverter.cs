using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// Converts a boolean to VerticalAlignment or HorizontalAlignment.
    /// True (stretch mode) = Stretch (fill available space)
    /// False (normal mode) = Top/Left (align to corner)
    /// </summary>
    public class BoolToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool stretch)
            {
                // When stretching, we want to fill the available space
                if (targetType == typeof(VerticalAlignment))
                {
                    return stretch ? VerticalAlignment.Stretch : VerticalAlignment.Top;
                }
                else if (targetType == typeof(HorizontalAlignment))
                {
                    return stretch ? HorizontalAlignment.Stretch : HorizontalAlignment.Left;
                }
            }

            // Default alignment
            if (targetType == typeof(VerticalAlignment))
                return VerticalAlignment.Top;
            else
                return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
