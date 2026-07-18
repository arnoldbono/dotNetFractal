using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;

namespace dotNetFractal.Uno
{
    public static class ConvertBitmapToImageSource
    {
        /// <summary>
        /// Converts a SKBitmap to an image that can be used as an ImageSource
        /// </summary>
        /// <param name="src">A SKBitmap image</param>
        /// <returns>The image as a BitmapImage for UNO</returns>
        public static ImageSource Clone(SKBitmap src)
        {
            using var image = SKImage.FromBitmap(src);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var ms = new MemoryStream();
            data.SaveTo(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(ms.AsRandomAccessStream());
            return bitmapImage;
        }

        /// <summary>
        /// Converts a SKBitmap to an image that can be used as an ImageSource.
        /// Uses WriteableBitmap for UNO Platform.
        /// </summary>
        /// <param name="src">A SKBitmap image</param>
        /// <returns>The image as a WriteableBitmap for UNO</returns>
        public static ImageSource ConvertFast(SKBitmap src)
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

        public static void Update(ImageSource imageSource, SKBitmap src, SKRectI targetRectangle, SKRectI sourceRect)
        {
            // Simplified implementation for UNO
            // Full implementation would require more complex pixel manipulation
            if (imageSource is WriteableBitmap writeableBitmap)
            {
                using var image = SKImage.FromBitmap(src);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                using var ms = new MemoryStream();
                data.SaveTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                writeableBitmap.SetSource(ms.AsRandomAccessStream());
            }
        }
    }
}

