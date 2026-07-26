using SkiaSharp;

namespace dotNetFractal.UI.Services;

/// <summary>
/// Platform-specific interface for converting bitmaps to ImageSource
/// </summary>
public interface IBitmapConverter
{
    object ConvertToImageSource(SKBitmap bitmap);

    /// <summary>
    /// Updates an existing ImageSource with new bitmap data without creating a new object.
    /// Returns true if successful, false if a new ImageSource needs to be created.
    /// </summary>
    bool TryUpdateImageSource(object? imageSource, SKBitmap bitmap);
}
