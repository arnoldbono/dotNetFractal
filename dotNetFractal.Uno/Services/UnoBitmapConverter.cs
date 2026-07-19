using dotNetFractal.UI.Services;
using SkiaSharp;

namespace dotNetFractal.Uno.Services;

/// <summary>
/// Uno-specific implementation of IBitmapConverter
/// </summary>
public class UnoBitmapConverter : IBitmapConverter
{
    public object ConvertToImageSource(SKBitmap bitmap)
    {
        return ConvertBitmapToImageSource.ConvertFast(bitmap);
    }
}
