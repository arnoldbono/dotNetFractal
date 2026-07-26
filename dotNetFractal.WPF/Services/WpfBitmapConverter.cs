using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        return (ImageSource)ConvertFast(bitmap);
    }

    /// <summary>
    /// Updates an existing WriteableBitmap in-place without creating a new object.
    /// This prevents flickering by reusing the same ImageSource object.
    /// </summary>
    public bool TryUpdateImageSource(object? imageSource, SKBitmap bitmap)
    {
        if (imageSource is not WriteableBitmap writeableBitmap)
            return false;

        if (writeableBitmap.PixelWidth != bitmap.Width || 
            writeableBitmap.PixelHeight != bitmap.Height)
            return false;

        Copy(writeableBitmap, bitmap);
        return true;
    }

    /// <summary>
    /// Converts a bitmap to an image that can be used as an ImageSource.
    /// Uses direct pixel copying for maximum performance.
    /// </summary>
    /// <param name="bitmap">A bitmap image</param>
    /// <returns>The image as a WriteableBitmap for WPF</returns>
    private static WriteableBitmap ConvertFast(SKBitmap bitmap)
    {
        var width = bitmap.Width;
        var height = bitmap.Height;

        var writeableBitmap = new WriteableBitmap(
            width,
            height,
            96, // DPI X
            96, // DPI Y
            PixelFormats.Pbgra32,
            null); // Palette

        Copy(writeableBitmap, bitmap);
        return writeableBitmap;
    }

    private static void Copy(WriteableBitmap writeableBitmap, SKBitmap bitmap)
    {
        var width = bitmap.Width;
        var height = bitmap.Height;

        writeableBitmap.Lock();
        try
        {
            var pixels = bitmap.GetPixels();
            var stride = width * 4;
            var bufferSize = stride * height;
            var dirtyRect = new Int32Rect(0, 0, width, height);

            // Copy directly to BackBuffer using unsafe code for maximum performance
            unsafe
            {
                Buffer.MemoryCopy(
                    (void*)pixels,
                    (void*)writeableBitmap.BackBuffer,
                    bufferSize,
                    bufferSize);
            }

            // Mark the entire bitmap as dirty to ensure WPF updates the display
            writeableBitmap.AddDirtyRect(dirtyRect);
        }
        finally
        {
            writeableBitmap.Unlock();
        }
    }

}
