using dotNetFractal.UI.Services;
using SkiaSharp;

namespace dotNetFractal.WPF.Services;

/// <summary>
/// WPF-specific implementation of IBitmapConverter
/// </summary>
public class WpfBitmapConverter : IBitmapConverter
{
    public object ConvertToImageSource(SKBitmap bitmap)
    {
        return ConvertBitmapToImageSource.ConvertFast(bitmap);
    }
}
