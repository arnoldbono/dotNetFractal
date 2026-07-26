using dotNetFractal.UI.Services;
using SkiaSharp;
using Microsoft.UI.Xaml.Media.Imaging;

namespace dotNetFractal.Uno.Services;

/// <summary>
/// Uno-specific implementation of IBitmapConverter
/// </summary>
public class UnoBitmapConverter : IBitmapConverter
{
    public object ConvertToImageSource(SKBitmap bitmap)
    {
        return (ImageSource)ConvertFast(bitmap);
    }

    /// <summary>
    /// Updates an existing WriteableBitmap in-place.
    /// For Uno, we need to re-encode and set the source.
    /// </summary>
    public bool TryUpdateImageSource(object? imageSource, SKBitmap bitmap)
    {
        if (imageSource is not WriteableBitmap writeableBitmap)
            return false;

        if (writeableBitmap.PixelWidth != bitmap.Width || 
            writeableBitmap.PixelHeight != bitmap.Height)
            return false;

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream();
        data.SaveTo(ms);
        ms.Seek(0, SeekOrigin.Begin);
        writeableBitmap.SetSource(ms.AsRandomAccessStream());

        return true;
    }

    /// <summary>
    /// Converts a SKBitmap to an image that can be used as an ImageSource.
    /// Uses WriteableBitmap for Uno Platform.
    /// </summary>
    /// <param name="src">A SKBitmap image</param>
    /// <returns>The image as a WriteableBitmap for Uno</returns>
    private static WriteableBitmap ConvertFast(SKBitmap src)
    {
        var width = src.Width;
        var height = src.Height;

        var writeableBitmap = new WriteableBitmap(width, height);

        using var image = SKImage.FromBitmap(src);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream();
        data.SaveTo(ms);
        ms.Seek(0, SeekOrigin.Begin);
        writeableBitmap.SetSource(ms.AsRandomAccessStream());

        return writeableBitmap;
    }
}
