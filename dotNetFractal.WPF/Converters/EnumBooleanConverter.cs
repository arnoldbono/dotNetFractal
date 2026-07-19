using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using dotNetFractal.UI.Models;

namespace dotNetFractal.WPF.Converters;

public class EnumBooleanConverter : IValueConverter
{
    private Type? _lastEnumType;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string parameterString && value != null)
        {
            // Cache the enum type for use in ConvertBack
            _lastEnumType = value.GetType();

            return Enum.IsDefined(value.GetType(), value) && 
                   Enum.Parse(value.GetType(), parameterString).Equals(value);
        }
        return value?.Equals(parameter) ?? false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        // Only update when RadioButton is checked (true), not when unchecked (false)
        if (value is bool boolValue && !boolValue)
        {
            return DependencyProperty.UnsetValue;
        }

        if (parameter is string parameterString)
        {
            // Try to use the cached enum type from the last Convert call
            Type? enumType = _lastEnumType;

            // If we don't have a cached type, try to find it by searching for an enum with this value
            if (enumType == null || !enumType.IsEnum)
            {
                enumType = FindEnumTypeByValue(parameterString);
            }

            if (enumType != null && enumType.IsEnum)
            {
                return Enum.Parse(enumType, parameterString);
            }
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, parameter.ToString()!);
        }

        return parameter;
    }

    private Type? FindEnumTypeByValue(string valueName)
    {
        // Search in the current app's assembly and dotNetFractal.WPF assembly
        var assemblies = new[]
        {
            Assembly.GetExecutingAssembly(),
            typeof(ResolutionEnum).Assembly
        };

        foreach (var assembly in assemblies.Distinct())
        {
            var enumType = assembly.GetTypes()
                .FirstOrDefault(t => t.IsEnum && Enum.GetNames(t).Contains(valueName));

            if (enumType != null)
            {
                return enumType;
            }
        }

        return null;
    }
}
