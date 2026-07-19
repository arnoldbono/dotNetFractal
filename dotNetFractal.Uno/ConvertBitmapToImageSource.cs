using SkiaSharp;
#if __IOS__ || __MACCATALYST__ || __ANDROID__ || __WASM__
using Microsoft.UI.Xaml.Media.Imaging;
#else // Windows/Desktop
using SkiaSharp.Views.Windows;
#endif

namespace dotNetFractal.Uno;

public static class ConvertBitmapToImageSource
{
    /// <summary>
    /// Converts a SKBitmap to an image that can be used as an ImageSource
    /// </summary>
    /// <param name="src">A SKBitmap image</param>
    /// <returns>The image as a BitmapImage for UNO</returns>
    public static ImageSource Clone(SKBitmap src)
    {
        return ConvertFast(src);
    }

    /// <summary>
    /// Converts a SKBitmap to an image that can be used as an ImageSource.
    /// Uses WriteableBitmap for UNO Platform.
    /// </summary>
    /// <param name="src">A SKBitmap image</param>
    /// <returns>The image as a WriteableBitmap for UNO</returns>
    public static ImageSource ConvertFast(SKBitmap src)
    {
#if __IOS__ || __MACCATALYST__ || __ANDROID__ || __WASM__
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
#else // Windows/Desktop
        return src.ToWriteableBitmap(); // Ensure the bitmap is in a format that can be used with WriteableBitmap
#endif
    }
}

