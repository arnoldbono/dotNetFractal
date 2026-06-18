using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace dotNetFractal.WPF
{
    public static class ConvertBitmapToImageSource
    {
        /// <summary>
        /// Converts a bitmap to an image that can be used as an ImageSource
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        public static ImageSource Clone(Bitmap src)
        {
            var ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        /// <summary>
        /// Converts a bitmap to an image that can be used as an ImageSource.
        /// Uses direct pixel copying for maximum performance.
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a WriteableBitmap for WPF</returns>
        public static ImageSource ConvertFast(Bitmap src)
        {
            var width = src.Width;
            var height = src.Height;
            var sourceRect = new Rectangle(0, 0, width, height);

            var writeableBitmap = new WriteableBitmap(
                width,
                height,
                96, // DPI X
                96, // DPI Y
                GetPixelFormat(src.PixelFormat),
                null); // Palette

            Update(writeableBitmap, src, sourceRect, new Int32Rect(0, 0, width, height));

            return writeableBitmap;
        }

        public static void Update(ImageSource imageSource, Bitmap src, Rectangle targetRectangle, Int32Rect sourceRect)
        {
            if (imageSource is not WriteableBitmap writeableBitmap)
            {
                throw new ArgumentException("ImageSource must be a WriteableBitmap", nameof(imageSource));
            }

            var bitmapData = src.LockBits(
                targetRectangle,
                ImageLockMode.ReadOnly,
                src.PixelFormat);

            try
            {
                writeableBitmap.WritePixels(
                    sourceRect,
                    bitmapData.Scan0,
                    bitmapData.Stride * writeableBitmap.PixelHeight,
                    bitmapData.Stride);
            }
            finally
            {
                src.UnlockBits(bitmapData);
            }
        }

        private static System.Windows.Media.PixelFormat GetPixelFormat(System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                System.Drawing.Imaging.PixelFormat.Format32bppArgb => PixelFormats.Bgra32,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb => PixelFormats.Bgr32,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb => PixelFormats.Bgr24,
                System.Drawing.Imaging.PixelFormat.Format8bppIndexed => PixelFormats.Gray8,
                _ => PixelFormats.Bgra32 // Default fallback
            };
        }
    }
}