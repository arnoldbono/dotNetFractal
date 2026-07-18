using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SkiaSharp;

namespace dotNetFractal.WPF;

public static class ConvertBitmapToImageSource
{
    /// <summary>
    /// Converts a bitmap to an image that can be used as an ImageSource.
    /// Uses direct pixel copying for maximum performance.
    /// </summary>
    /// <param name="src">A bitmap image</param>
    /// <returns>The image as a WriteableBitmap for WPF</returns>
    public static ImageSource ConvertFast(SKBitmap src)
    {
        var width = src.Width;
        var height = src.Height;

        var writeableBitmap = new WriteableBitmap(
            width,
            height,
            96, // DPI X
            96, // DPI Y
            PixelFormats.Pbgra32,
            null); // Palette

        writeableBitmap.Lock();
        try
        {
            var pixels = src.GetPixels();
            var stride = width * 4;
            writeableBitmap.WritePixels(
                new Int32Rect(0, 0, width, height),
                pixels,
                stride * height,
                stride);
        }
        finally
        {
            writeableBitmap.Unlock();
        }

        return writeableBitmap;
    }
}
