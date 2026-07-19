using SkiaSharp;

namespace dotNetFractal.UI.Services;

/// <summary>
/// Platform-specific interface for converting bitmaps to ImageSource
/// </summary>
public interface IBitmapConverter
{
    object ConvertToImageSource(SKBitmap bitmap);
}
